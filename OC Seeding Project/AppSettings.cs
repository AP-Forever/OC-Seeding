using OrderCloud.SDK;

//namespace OC_Seeding_Project;

public class AppSettings
{
    public OrderCloudSettings OrderCloudSettings { get; set; } = new OrderCloudSettings();
}

public class OrderCloudSettings
{
    public string ApiUrl { get; set; } = string.Empty;
    public string AuthUrl { get; set; } = string.Empty;

    public string MiddlewareClientID { get; set; } = string.Empty;
    public string MiddlewareClientSecret { get; set; } = string.Empty;

    public SettingsConstants SettingsConstants { get; set; } = new SettingsConstants();
}

public class SettingsConstants
{
    public string WebhookHashKey { get; set; } = string.Empty;
    public string IncrementorPrefix { get; set; } = string.Empty;

    // Seller
    public string SellerSecurityProfileId {get; set;} = string.Empty;
    public string SellerSecurityProfileName { get; set; } = string.Empty;
    public ICollection<ApiRole> SellerSecurityProfileRoles { get; set; } = new List<ApiRole>();
    public string SellerClientId { get; set; } = string.Empty;
    public string SellerClientSecret { get; set; } = string.Empty;
    public string SellerClientName { get; set; } = string.Empty;
    public string SellerUserId { get; set; } = string.Empty;
    public string SellerUsername { get; set; } = string.Empty;
    public string SellerUserEmail { get; set; } = string.Empty;
    public string SellerUserFirstName { get; set; } = string.Empty;
    public string SellerUserLastName { get; set; } = string.Empty;

    // Buyer
    public string BuyerSecurityProfileId { get; set; } = string.Empty;
    public string BuyerSecurityProfileName { get; set; } = string.Empty;
    public ICollection<ApiRole> BuyerSecurityProfileRoles { get; set; } = new List<ApiRole>();
    public string BuyerClientId { get; set; } = string.Empty;
    public string BuyerClientSecret { get; set; } = string.Empty;
    public string BuyerClientName { get; set; } = string.Empty;
    public string BuyerId { get; set; } = string.Empty;
    public string BuyerName { get; set; } = string.Empty;
    public string BuyerUserId { get; set; } = string.Empty;
    public string BuyerUsername { get; set; } = string.Empty;
    public string BuyerUserEmail { get; set; } = string.Empty;
    public string BuyerUserFirstName { get; set; } = string.Empty;
    public string BuyerUserLastName { get; set; } = string.Empty;

    // Supplier
    public string SupplierSecurityProfileId { get; set; } = string.Empty;
    public string SupplierSecurityProfileName { get; set; } = string.Empty;
    public ICollection<ApiRole> SupplierSecurityProfileRoles { get; set; } = new List<ApiRole>();
    public string SupplierClientId { get; set; } = string.Empty;
    public string SupplierClientSecret { get; set; } = string.Empty;
    public string SupplierClientName { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierUserId { get; set; } = string.Empty;
    public string SupplierUsername { get; set; } = string.Empty;
    public string SupplierUserEmail { get; set; } = string.Empty;
    public string SupplierUserFirstName { get; set; } = string.Empty;
    public string SupplierUserLastName { get; set; } = string.Empty;

    // Catalog
    public string CatalogId { get; set; } = string.Empty;
    public string CatalogName { get; set; } = string.Empty;
    public string FullAccessSecurityProfile { get; set; } = string.Empty;
    public string DefaultLocationID { get; set; } = string.Empty;
    public string AllowedSecretChars { get; set; } = string.Empty;

    //Integrations Middleware
    public string IntegrationsClientName { get; set; } = string.Empty;
    public string IntegrationsUserId { get; set; } = string.Empty;
    public string IntegrationsUsername { get; set; } = string.Empty;
    public string IntegrationsUserEmail { get; set; } = string.Empty;
    public string IntegrationsUserFirstName { get; set; } = string.Empty;
    public string IntegrationsUserLastName { get; set; } = string.Empty;

    //Integration Event
    public string CheckoutIntegrationEventId { get; set; } = string.Empty;
    public string CheckoutIntegrationEventName { get; set; } = string.Empty;
}