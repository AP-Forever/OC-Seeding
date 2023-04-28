using Flurl.Http.Configuration;
using Flurl.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System.Net;
using OC_Seeding_Project.Services;
using OC_Seeding_Project.API.Commands;

namespace OC_Seeding_Project
{
    public class Startup
    {
        private readonly AppSettings _settings;

        public Startup(AppSettings settings)
        {
            _settings = settings;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            
            var flurlClientFactory = new PerBaseUrlFlurlClientFactory();
            //services.AddMvc(options => options.EnableEndpointRouting = false);
            services
                .AddCors(o => o.AddPolicy("integrationcors", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }))
                .Configure<KestrelServerOptions>(options =>
                {
                    options.AllowSynchronousIO = true;
                })
                .Configure<IISServerOptions>(options =>
                {
                    options.AllowSynchronousIO = true;
                })
                .ConfigureServices()
                .AddOrderCloudUserAuth()
                .AddOrderCloudWebhookAuth(opts => opts.HashKey = _settings.OrderCloudSettings.SettingsConstants.WebhookHashKey)
                .Inject<IPortalService>()
                .Inject<ISeedCommand>()
                .AddSingleton<IFlurlClientFactory>(x => flurlClientFactory)
                .AddSingleton<IOrderCloudClient>(provider => new OrderCloudClient(new OrderCloudClientConfig
                {
                    ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                    AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                    ClientId = _settings.OrderCloudSettings.MiddlewareClientID,
                    ClientSecret = _settings.OrderCloudSettings.MiddlewareClientSecret,
                    Roles = new[]
                    {
                        ApiRole.FullAccess
                    }
                }));
            var serviceProvider = services.BuildServiceProvider();


            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCatalystExceptionHandler();
            app.UseHttpsRedirection();
            app.UseCors("integrationcors");
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
