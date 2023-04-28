using Flurl.Http.Configuration;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace OC_Seeding_Project 
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Links to an Azure App Configuration resource that holds the app settings.
            // Set this in your visual studio Env Variables.
            var appConfigConnectionString = Environment.GetEnvironmentVariable("APP_CONFIG_CONNECTION");

            WebHost.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (appConfigConnectionString != null)
                    {
                        config.AddAzureAppConfiguration(appConfigConnectionString);
                    }
                    config.AddJsonFile("appSettings.json", optional: true);
                })
                .UseStartup<Startup>()
                .ConfigureServices((ctx, services) =>
                {
                    services.Configure<AppSettings>(ctx.Configuration);
                    services.AddTransient(sp => sp.GetService<IOptionsSnapshot<AppSettings>>().Value);
                })
                .Build()
                .Run();
        }
    }
}

//var builder = WebApplication.CreateBuilder(args);

//IServiceCollection services = builder.Services;
//services.Configure<AppSettings>(builder.Configuration);



//IConfigurationSection ocSettings = builder.Configuration.GetSection("OrderCloudSettings");
//var clientConfig = new OrderCloudClientConfig
//{
//    ApiUrl = ocSettings.GetValue<string>("ApiUrl"),
//    AuthUrl = ocSettings.GetValue<string>("AuthUrl"),
//    ClientId = ocSettings.GetValue<string>("MiddlewareSecretClientId"),
//    ClientSecret = ocSettings.GetValue<string>("MiddlewareSecretClientSecret"),
//    GrantType = GrantType.ClientCredentials,
//    Roles = new[] { ApiRole.FullAccess }
//};

//var flurlClientFactory = new PerBaseUrlFlurlClientFactory();

//services.AddLogging();
//services.AddControllers();
//services.AddCors();
//services.AddSingleton<IFlurlClientFactory>(x => flurlClientFactory);
//services.AddSingleton<IOrderCloudClient, OrderCloudClient>(provider => new OrderCloudClient(clientConfig));
//services.AddScoped<ISeedCommand, SeedCommand>();
//services.AddSingleton<IPortalService, PortalService>();
//services.AddTransient(sp => sp.GetService<IOptionsSnapshot<AppSettings>>()?.Value);

//var app = builder.Build();

//app.UseCors();

//app.Run();