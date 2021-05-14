using System;
using System.Collections.Generic;
using System.Text;

namespace DiStock.DAL.Datas
{
    public class ProjectInvitationData
    {
        public int UserAuthorId { get; set; }

        public string UserAuthorPseudo { get; set; }

        public int UserInvitedId { get; set; }

        public string UserInvitedPseudo { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }
    }
}
