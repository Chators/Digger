using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models.Project
{
    public class UnassignUserToProjectViewModel
    {
        public int UserId { get; set; }

        public int ProjectId { get; set; }
    }
}
