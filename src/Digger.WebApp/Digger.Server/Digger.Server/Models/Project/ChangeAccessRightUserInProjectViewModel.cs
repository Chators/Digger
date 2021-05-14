using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models.Project
{
    public class ChangeAccessRightUserInProjectViewModel
    {
        public int UserId { get; set; }

        public int ProjectId { get; set; }

        public int AccessRightId { get; set; }
    }
}
