namespace Digger.Server.Models.ResearchModule
{
    public class UpdateResearchModuleViewModel
    {
        public int Id { get; set; }

        public int FktStaticEntity { get; set; }

        public int FktStaticFootprint { get; set; }
    
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
