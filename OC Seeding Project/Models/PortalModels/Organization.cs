using THR_Seeding_Project.Models.PortalModels;

namespace OC_Seeding_Project.Models.PortalModels
{
    public class Organization
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public PortalUser Owner { get; set; }
        public string Environment { get; set; }
        public readonly OrgRegion Region = new OrgRegion();
    }
}
