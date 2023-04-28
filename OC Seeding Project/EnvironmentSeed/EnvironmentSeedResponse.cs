namespace OC_Seeding_Project.EnvironmentSeed
{
    public class EnvironmentSeedResponse
    {
        public string Comments { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationID { get; set; }
        public string OrderCloudEnvironment { get; set; }
        public Dictionary<string, dynamic> ApiClients { get; set; }
    }
}
