using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models.Project
{
    public class SendInvitationProjectViewModel
    {
        public int UserAuthorId { get; set; }

        public int UserInvitedId { get; set; }

        public int ProjectId { get; set; }
    }
}
