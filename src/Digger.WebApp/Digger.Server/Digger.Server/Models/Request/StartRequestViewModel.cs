using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models.Request
{
    public class StartRequestViewModel
    {
        public int RequestId { get; set; }

        public string UidNode { get; set; }

        public List<StartRequestSoftwareViewModel> Softwares { get; set; }

        public string DataEntity { get; set; }
    }
}
