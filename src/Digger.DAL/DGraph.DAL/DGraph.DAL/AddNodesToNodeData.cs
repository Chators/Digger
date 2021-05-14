using System;
using System.Collections.Generic;
using System.Text;

namespace DGraph.DAL
{
    public class AddNodesToNodeData
    {
        public string ProjectId { get; set; }

        public string Author { get; set; }
        
        public string Data { get; set; }

        public string TypeOfData { get; set; }

        public string Source { get; set; }

        public string Note { get; set; }

        public string LastUpdate { get; set; }

        public List<Link> Link { get; set; }
    }
}
