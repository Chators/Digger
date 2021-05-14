namespace Digger.Server.Models.Project
{
    public class UpdateProjectViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte IsPublic { get; set; }
    }
}
