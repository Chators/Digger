using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models
{
    public class SoftwareUpdateViewModel
    {
        public int Id { get; set; }

        public string NewName { get; set; }

        public string Description { get; set; }
    }
}
