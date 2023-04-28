using System.ComponentModel.DataAnnotations;

namespace OC_Seeding_Project.EnvironmentSeed
{
    public class EnvironmentSeedRequestModel
    {
        #region Required settings

        /// <summary>
        /// The username for logging in to https://portal.ordercloud.io
        /// </summary>
        [Required]
        public string PortalUsername { get; set; }

        /// <summary>
        /// The password for logging in to https://portal.ordercloud.io
        /// </summary>
        [Required]
        public string PortalPassword { get; set; }

        /// <summary>
        /// The url to your hosted middleware endpoint
        /// needed for webhooks and message senders
        /// </summary>
        [Required]
        public string MiddlewareBaseUrl { get; set; }

        /// <summary>
        /// Container for OrderCloud Settings
        /// </summary>
        [Required]
        public OrderCloudSeedSettings OrderCloudSettings { get; set; }

        /// <summary>
        /// Marketplace Name
        /// </summary>
        [Required]
        public string MarketplaceName { get; set; }

        /// <summary>
        /// Marketplace ID
        /// </summary>
        public string? MarketplaceID { get; set; }

        /// <summary>
		/// Defaults to false
		/// Enables anonymous shopping whereby users do not have to be logged in to view products or submit an order
		/// pricing and visibility will be determined by what the default user can see
		/// </summary>
		public bool EnableAnonymousShopping { get; set; } = false;

        #endregion
    }

    public class OrderCloudSeedSettings
    {
        /// <summary>
        /// The ordercloud environment
        /// </summary>
        [Required]
        public string Environment { get; set; }

        /// <summary>
        /// Used to secure your webhook endpoints
        /// provide a secure, non-guessable string
        /// </summary>
        [Required, MaxLength(15)]
        public string WebhookHashKey { get; set; }
    }

    public class OcEnv
    {
        public string environmentName { get; set; }
        public string apiUrl { get; set; }
    }

    public class OrderCloudEnvironments
    {
        public static OcEnv Production = new OcEnv()
        {
            environmentName = "Production",
            apiUrl = "https://api.ordercloud.io"
        };
        public static OcEnv Sandbox = new OcEnv()
        {
            environmentName = "Sandbox",
            apiUrl = "https://sandboxapi.ordercloud.io"
        };
    }
}
