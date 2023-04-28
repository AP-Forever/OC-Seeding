using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OrderCloud.Catalyst;

namespace OC_Seeding_Project
{
    public static class CatalystConfigureWebApiServices
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddMvc(o =>
            {
                //o.Filters.Add(new Common.Attributes.ValidateModelAttribute());
                o.EnableEndpointRouting = false;
            });
            services.AddControllers().ConfigureApiBehaviorOptions(delegate (ApiBehaviorOptions o)
            {
                o.SuppressModelStateInvalidFilter = true;
            }).AddNewtonsoftJson(delegate (MvcNewtonsoftJsonOptions options)
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddMvc().AddNewtonsoftJson(delegate (MvcNewtonsoftJsonOptions options)
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
            services.AddMvc(delegate (MvcOptions o)
            {
                o.Filters.Add(new ValidateModelAttribute());
                o.EnableEndpointRouting = false;
            });
            services.AddCors(delegate (CorsOptions o)
            {
                o.AddPolicy("integrationcors", delegate (CorsPolicyBuilder builder)
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            return services;
        }

        public static IServiceCollection AddOrderCloudUserAuth(this IServiceCollection services)
        {
            services.AddSingleton<ISimpleCache, LazyCacheService>().AddAuthentication().AddScheme<OrderCloudUserAuthOptions, OrderCloudUserAuthHandler>("OrderCloudUser", null);
            return services;
        }

        public static IServiceCollection AddOrderCloudWebhookAuth(this IServiceCollection services, Action<OrderCloudWebhookAuthOptions> configureOptions)
        {
            services.AddAuthentication().AddScheme<OrderCloudWebhookAuthOptions, OrderCloudWebhookAuthHandler>("OrderCloudWebhook", null, configureOptions);
            return services;
        }
    }
}
