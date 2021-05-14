using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models
{
    public class AssignUserToProject
    {
        public int UserId { get; set; }

        public int ProjectId { get; set; }
    }
}
