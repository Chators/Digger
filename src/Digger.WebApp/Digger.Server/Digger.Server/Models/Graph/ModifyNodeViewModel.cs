using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models.Graph
{
    public class ModifyNodeViewModel
    {
        public string Uid { get; set; }

        public string Data { get; set; }

        public string Note { get; set; }

        public string Source { get; set; }

        public string TypeOfData { get; set; }
    }
}
