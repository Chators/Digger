using System;

namespace DiStock.DAL
{
    public class ProjectData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte IsPublic { get; set; }

        public DateTime Date { get; set; }
    }
}
