using System;
using System.Collections.Generic;
using System.Text;

namespace DGraph.DAL
{
    public class NodeSearchData
    {
        public string Data { get; set; }

        public string TypeOfData { get; set; }

        public string Source { get; set; }

        public string Note { get; set; }

        public List<Link> Link { get; set; }
    }
}
