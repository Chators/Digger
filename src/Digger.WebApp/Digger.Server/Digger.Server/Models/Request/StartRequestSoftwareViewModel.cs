using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models.Request
{
    public class StartRequestSoftwareViewModel
    {
        public string Name { get; set; }

        public List<string> ResearchModules { get; set; }
    }
}
