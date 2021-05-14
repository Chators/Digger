using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models
{
    public class CreateProjectViewModel
    {
        public int FktUser { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte IsPublic { get; set; }
    }
}
