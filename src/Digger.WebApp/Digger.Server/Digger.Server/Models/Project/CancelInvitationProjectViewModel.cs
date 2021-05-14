using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models.Project
{
    public class CancelInvitationProjectViewModel
    {
        public int UserAuthorId { get; set; }

        public int UserInvitedId { get; set; }

        public int ProjectId { get; set; }
    }
}
