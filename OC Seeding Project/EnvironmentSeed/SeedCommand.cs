using System.Net;
using System.Runtime;
using Azure.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using OC_Seeding_Project.EnvironmentSeed;
using OC_Seeding_Project.Models.PortalModels;
using OC_Seeding_Project.Services;
using static OrderCloud.SDK.ErrorCodes;
using static OrderCloud.SDK.WebhookPayloads;
using static OC_Seeding_Project.EnvironmentSeed.SeedConstants;
using ApiClient = OrderCloud.SDK.ApiClient;
using Buyer = OrderCloud.SDK.Buyer;
using Catalog = OrderCloud.SDK.Catalog;
using IntegrationEvent = OrderCloud.SDK.IntegrationEvent;
using SecurityProfile = OrderCloud.SDK.SecurityProfile;
using User = OrderCloud.SDK.User;

namespace OC_Seeding_Project.API.Commands
{
    public interface ISeedCommand
    {
        Task<EnvironmentSeedResponse> Seed(EnvironmentSeedRequestModel seed);
    }

    public class SeedCommand : ISeedCommand
    {
        private IOrderCloudClient _client;

        private readonly IPortalService _portal;

        private readonly SettingsConstants _settings;

        private readonly ILogger<SeedCommand> _logger;

        public SeedCommand(
            IOrderCloudClient client,
            IOptions<AppSettings> config,
            IPortalService portal,
            ILogger<SeedCommand> logger)
        {
            _client = client;
            _settings = config.Value.OrderCloudSettings.SettingsConstants;
            _logger = logger;
            _portal = portal;
        }

        public async Task<EnvironmentSeedResponse> Seed(EnvironmentSeedRequestModel seed)
        {
            OcEnv requestedEnv = ValidateEnvironment(seed.OrderCloudSettings.Environment);

            if (requestedEnv.environmentName == OrderCloudEnvironments.Production.environmentName)
            {
                throw new Exception("Cannot create a production environment via the environment seed endpoint. Please contact an OrderCloud Developer to create a production org.");
            }

            // lets us handle requests to multiple api environments
            _client = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = requestedEnv.apiUrl,
                AuthUrl = requestedEnv.apiUrl
            });

            var portalUserToken = await _portal.Login(seed.PortalUsername, seed.PortalPassword);
            var sellerOrg = await GetOrCreateOrg(portalUserToken, requestedEnv.environmentName, seed.MarketplaceName, seed.MarketplaceID);
            var orgToken = await _portal.GetOrgToken(sellerOrg.Id, portalUserToken);

            // Seller
            await CreateOrUpdateDefaultSellerUser(orgToken);

            // Buyer
            await SeedBuyer(orgToken);
            await SeedBuyerUser(orgToken);
            await CreateOrUpdateSecurityProfiles(orgToken);


            // Suppliers
            await SeedSupplier(orgToken);
            await SeedSupplierUser(orgToken);
            await SeedSupplierSecurityProfile(orgToken);

            // Catalog
            await SeedCatalog(orgToken);
            await SeedCatalogAssignments(orgToken);

            var apiClients = new ApiClients
            {
                SellerApiClient = await CreateOrGetApiClient(_settings.SellerClientName, clientId: _settings.SellerClientId, clientSecret: _settings.SellerClientSecret, token: orgToken, allowAnySeller: true),
                BuyerApiClient = await CreateOrGetApiClient(_settings.BuyerClientName, clientId: _settings.BuyerClientId, clientSecret: _settings.BuyerClientSecret, token: orgToken, enableAnonymousShopping: seed.EnableAnonymousShopping, allowAnyBuyer: true),
                SupplierApiClient = await CreateOrGetApiClient(_settings.SupplierClientName, clientId: _settings.SupplierClientId, clientSecret: _settings.SupplierClientSecret, token: orgToken, allowAnySupplier: true),
                MiddlewareApiClient = await CreateOrGetApiClient(_settings.IntegrationsClientName, clientSecret: Guid.NewGuid().ToString().Replace("-",""), token: orgToken, allowAnyBuyer: true, allowAnySupplier: true, allowAnySeller: true)
            };

            await CreateOrUpdateSecurityProfileAssignments(orgToken);
            await CreateOrUpdateAndAssignIntegrationEvents(orgToken, apiClients.BuyerApiClient, seed);

            _logger.LogInformation("Success! Your environment is now seeded. The following clientIDs & secrets should be used to finalize the configuration of your application. The initial admin username and password can be used to sign into your admin application: \n");

            return new EnvironmentSeedResponse
            {
                Comments = "Success! Your environment is now seeded. The following clientIDs & secrets should be used to finalize the configuration of your application. The initial admin username and password can be used to sign into your admin application",
                OrganizationName = sellerOrg.Name,
                OrganizationID = sellerOrg.Id,
                OrderCloudEnvironment = requestedEnv.environmentName,
                ApiClients = new Dictionary<string, dynamic>
                {
                    ["Middleware"] = new
                    {
                        ClientID = apiClients.MiddlewareApiClient.ID,
                        ClientSecret = apiClients.MiddlewareApiClient.ClientSecret
                    },
                    ["Seller"] = new
                    {
                        ClientID = apiClients.SellerApiClient.ID
                    },
                    ["Buyer"] = new
                    {
                        ClientID = apiClients.BuyerApiClient.ID
                    },
                    ["Supplier"] = new
                    {
                        ClientID = apiClients.SupplierApiClient.ID
                    }
                }
            };

        }

        public class ApiClients
        {
            public ApiClient SellerApiClient { get; set; }
            public ApiClient BuyerApiClient { get; set; }
            public ApiClient SupplierApiClient { get; set; }
            public ApiClient MiddlewareApiClient { get; set; }
        }

        private async Task CreateOrUpdateAndAssignIntegrationEvents(string token, ApiClient buyerApiClient, EnvironmentSeedRequestModel seed = null)
        {

            if (string.IsNullOrEmpty(seed.MiddlewareBaseUrl)) return;

            // this gets called by both the /seed command and the post-staging restore so we need to handle getting settings from two sources
            var middlewareBaseUrl = seed.MiddlewareBaseUrl;
            var webhookHashKey = seed.OrderCloudSettings.WebhookHashKey;
            var checkoutEvent = new IntegrationEvent()
            {
                ElevatedRoles = new[] { ApiRole.FullAccess },
                ID = _settings.CheckoutIntegrationEventId,
                EventType = IntegrationEventType.OrderCheckout,
                Name = _settings.CheckoutIntegrationEventName,
                CustomImplementationUrl = middlewareBaseUrl,
                HashKey = webhookHashKey
            };
            await _client.IntegrationEvents.SaveAsync(checkoutEvent.ID, checkoutEvent, token);

            await _client.ApiClients.PatchAsync(buyerApiClient.ID, new PartialApiClient { OrderCheckoutIntegrationEventID = _settings.CheckoutIntegrationEventId }, token);
        }

        private async Task CreateOrUpdateSecurityProfileAssignments(string orgToken)
        {
            // assign buyer security profiles
            await _client.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment
            {
                BuyerID = _settings.BuyerId,
                SecurityProfileID = CustomRole.BaseBuyer.ToString()
            }, orgToken);

            // assign seller security profiles to seller org
            foreach(var role in SellerRoles)
            {
                try
                {
                    await _client.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
                    {
                        SecurityProfileID = role.ToString()
                    }, orgToken);
                }
                catch(OrderCloudException oce)
                {
                    _logger.LogWarning($"Error assigning role {role.ToString()}: {oce.Message}");
                }
            }

            // assign full access security profile to default admin user
            var adminUsersList = await _client.AdminUsers.ListAsync(filters: new { Username = _settings.SellerUsername }, accessToken: orgToken);
            var defaultAdminUser = adminUsersList.Items.FirstOrDefault();
            if (defaultAdminUser == null)
            {
                throw new Exception($"Unable to find default admin user (username: {_settings.SellerUsername}");
            }

            await _client.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
            {
                SecurityProfileID = _settings.FullAccessSecurityProfile,
                UserID = defaultAdminUser.ID
            }, orgToken);

            // assign full access security profile to default admin user
            adminUsersList = await _client.AdminUsers.ListAsync(filters: new { Username = _settings.IntegrationsUsername }, accessToken: orgToken);
            var integrationsAdminUser = adminUsersList.Items.FirstOrDefault();
            if (integrationsAdminUser == null)
            {
                throw new Exception($"Unable to find default integrations admin user (username: {_settings.IntegrationsUsername}");
            }

            await _client.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
            {
                SecurityProfileID = _settings.FullAccessSecurityProfile,
                UserID = integrationsAdminUser.ID
            }, orgToken);
        }

        private OcEnv ValidateEnvironment(string environment)
        {
            if (environment.ToLower() == "production")
            {
                return OrderCloudEnvironments.Production;
            }
            else if (environment.ToLower() == "sandbox")
            {
                return OrderCloudEnvironments.Sandbox;
            }
            else return null;
        }

        public async Task<Organization> GetOrCreateOrg(string token, string env, string marketplaceName, string? marketplaceID = null)
        {
            if (marketplaceID != null && !string.IsNullOrEmpty(marketplaceID))
            {
                var org = await VerifyOrgExists(marketplaceID, token);
                return org;
            }
            else
            {
                var org = new Organization()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Environment = env,
                    Name = marketplaceName == null ? "My Test Organization" : marketplaceName
                };
                try
                {
                    await _portal.GetOrganization(org.Id, token);
                    return await GetOrCreateOrg(token, env, marketplaceName, marketplaceID);
                }
                catch (Exception ex)
                {
                    await _portal.CreateOrganization(org, token);
                    return await _portal.GetOrganization(org.Id, token);
                }
            }
        }

        public async Task<Organization> VerifyOrgExists(string orgID, string devToken)
        {
            try
            {
                return await _portal.GetOrganization(orgID, devToken);
            }
            catch
            {
                // the portal API no longer allows us to create a production organization outside of portal
                // though its possible to create on sandbox - for consistency sake we'll require its created before seeding
                throw new Exception("Failed to retrieve seller organization with SellerOrgID. The organization must exist before it can be seeded");
            }
        }

        private async Task CreateOrUpdateDefaultSellerUser(string token)
        {
            // the middleware api client will use this user as the default context user
            var middlewareIntegrationsUser = new User
            {
                ID = _settings.IntegrationsUserId,
                Username = _settings.IntegrationsUsername,
                Email = _settings.IntegrationsUserEmail,
                FirstName = _settings.IntegrationsUserFirstName,
                LastName = _settings.IntegrationsUserLastName,
                Active = true,
                DateCreated = DateTime.Now
            };

            await _client.AdminUsers.SaveAsync(middlewareIntegrationsUser.ID, middlewareIntegrationsUser, token);

            // the seller api client will use this user as the default context user
            var defaultAdminUser = new User
            {
                ID = _settings.SellerUserId,
                Username = _settings.SellerUsername,
                Email = _settings.SellerUserEmail,
                FirstName = _settings.SellerUserFirstName,
                LastName = _settings.SellerUserLastName,
                Active = true,
                DateCreated = DateTime.Now
            };
            await _client.AdminUsers.SaveAsync(defaultAdminUser.ID, defaultAdminUser, token);
        }

        private async Task SeedBuyer(string token)
        {
            var seedBuyer = new Buyer
            {
                ID = _settings.BuyerId,
                Name = _settings.BuyerName,
                Active = true
            };

            await _client.Buyers.SaveAsync(_settings.BuyerId, seedBuyer, token);
        }

        private async Task SeedBuyerUser(string token)
        {
            User seedUser = new User
            {
                ID = _settings.BuyerUserId,
                Username = _settings.BuyerUsername,
                Email = _settings.BuyerUserEmail,
                FirstName = _settings.BuyerUserFirstName,
                LastName = _settings.BuyerUserLastName,
                Active = true,
                DateCreated = DateTime.Now
            };

            try
            {
                await _client.Users.SaveAsync(_settings.BuyerId, _settings.BuyerUserId, seedUser, token);
            }
            catch (OrderCloudException ex)
            {
                _logger.LogDebug(ex, ex.Message);
            }
        }

        private async Task CreateOrUpdateSecurityProfiles(string token)
        {
            try
            {
                var profiles = DefaultSecurityProfiles.Select(p =>
                    new SecurityProfile()
                    {
                        Name = p.ID.ToString(),
                        ID = p.ID.ToString(),
                        CustomRoles = p.CustomRoles.Select(r => r.ToString()).ToList(),
                        Roles = p.Roles
                    }).ToList();

                profiles.Add(new SecurityProfile()
                {
                    Roles = new List<ApiRole> { ApiRole.FullAccess },
                    Name = _settings.FullAccessSecurityProfile,
                    ID = _settings.FullAccessSecurityProfile
                });

                foreach(var profile in profiles)
                {
                    try
                    {
                        await _client.SecurityProfiles.SaveAsync(profile.ID, profile, token);
                    }
                    catch(OrderCloudException oce)
                    {
                        if(oce.HttpStatus == HttpStatusCode.Conflict) {
                            _logger.LogWarning($"Security Profile Exists: {profile.Name}");
                        }
                    }
                }
            }
            catch (OrderCloudException oce)
            {
                if (oce.HttpStatus == HttpStatusCode.Conflict) { }
            }
        }

        private async Task SeedSupplier(string token)
        {
            var seedSupplier = new Supplier
            {
                ID = _settings.SupplierId,
                Name = _settings.SupplierName,
                Active = true
            };

            await _client.Suppliers.SaveAsync(_settings.SupplierId, seedSupplier, token);
        }

        private async Task SeedSupplierUser(string token)
        {
            var seedUser = new User
            {
                ID = _settings.SupplierId,
                Username = _settings.SupplierUsername,
                Email = _settings.SupplierUserEmail,
                FirstName = _settings.SupplierUserFirstName,
                LastName = _settings.SupplierUserLastName,
                Active = true,
                DateCreated = DateTime.Now
            };

            try
            {
                await _client.SupplierUsers.SaveAsync(_settings.SupplierId, _settings.SupplierUserId, seedUser, token);
            }
            catch (OrderCloudException ex)
            {
                _logger.LogDebug(ex, ex.Message);
            }
        }

        private async Task SeedSupplierSecurityProfile(string token)
        {
            var securityProfile = new SecurityProfile
            {
                ID = _settings.SupplierSecurityProfileId,
                Name = _settings.SupplierSecurityProfileName,
                Roles = _settings.SupplierSecurityProfileRoles.ToList()
            };

            await _client.SecurityProfiles.SaveAsync(_settings.SupplierSecurityProfileId, securityProfile, token);
            await _client.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment
            {
                SecurityProfileID = _settings.SupplierSecurityProfileId,
                SupplierID = _settings.SupplierId
            }, token);
        }

        private async Task SeedCatalog(string token)
        {
            var catalog = new Catalog
            {
                ID = _settings.CatalogId,
                Name = _settings.CatalogName,
                Active = true
            };

            await _client.Catalogs.SaveAsync(_settings.CatalogId, catalog, token);
        }

        private async Task SeedCatalogAssignments(string token)
        {
            var catalogAssignment = new CatalogAssignment
            {
                BuyerID = _settings.BuyerId,
                CatalogID = _settings.CatalogId,
                ViewAllCategories = true,
                ViewAllProducts = true
            };

            await _client.Catalogs.SaveAssignmentAsync(catalogAssignment, token);
        }

        private async Task<ApiClient> CreateOrGetApiClient(
            string clientName, string token, string clientId = "", 
            string clientSecret = "", bool enableAnonymousShopping = false,
            bool allowAnyBuyer = false, bool allowAnySeller = false, bool allowAnySupplier = false)
        {
            var seedClient = new ApiClient
            {
                AppName = clientName,
                ID = clientId,
                ClientSecret = clientSecret,
                AccessTokenDuration = 600,
                Active = true,
                AllowAnyBuyer = allowAnyBuyer,
                AllowAnySupplier = allowAnySupplier,
                AllowSeller = allowAnySeller
            };

            if (enableAnonymousShopping)
            {
                seedClient.IsAnonBuyer = enableAnonymousShopping;
                seedClient.DefaultContextUserName = _settings.BuyerUsername;
            }

            try
            {
                var partialSeedClient = new PartialApiClient
                {
                    AppName = clientName,
                    ID = clientId,
                    ClientSecret = clientSecret,
                    AccessTokenDuration = 600,
                    Active = true,
                    AllowAnyBuyer = allowAnyBuyer,
                    AllowAnySupplier = allowAnySupplier,
                    AllowSeller = allowAnySeller
                };
                if (enableAnonymousShopping)
                {
                    partialSeedClient.IsAnonBuyer = enableAnonymousShopping;
                    partialSeedClient.DefaultContextUserName = _settings.BuyerUsername;
                }
                var existingClient = (await _client.ApiClients.ListAllAsync(filters: $"AppName={clientName}", token))?.FirstOrDefault();
                if(existingClient != null)
                {
                    return await _client.ApiClients.PatchAsync(existingClient.ID, partialSeedClient, token);
                }
                return await _client.ApiClients.CreateAsync(seedClient, token);
            }
            catch (OrderCloudException ex)
            {
                if(ex.HttpStatus == HttpStatusCode.NotFound)
                {
                    return await _client.ApiClients.CreateAsync(seedClient, token);
                }
                throw;
            }
        }
    }
}