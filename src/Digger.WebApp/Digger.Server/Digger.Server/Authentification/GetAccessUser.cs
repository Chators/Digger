using Digger.Server.Models.Project;
using DiStock.DAL;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Digger.Server.Authentification
{
    public class GetAccessUser
    {
        UserGateway _userGateway;
        ProjectGateway _projectGateway;

        public GetAccessUser(UserGateway userGateway, ProjectGateway projectGateway)
        {
            _userGateway = userGateway;
            _projectGateway = projectGateway;
        }

        public async Task<EnumProjectAccessRight> GetUserAccessRightProject(int userId, int projectId)
        {
            Result<string> result = await _projectGateway.GetUserAccessRightProject(userId, projectId);
            if (result.Content == null) return EnumProjectAccessRight.None;
            EnumProjectAccessRight projectAccessRight = (EnumProjectAccessRight)Enum.Parse(typeof(EnumProjectAccessRight), result.Content);
            return projectAccessRight;
        }

        public bool UserCookieIs(HttpContext httpContext, string targetUserId)
        {
            return httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) == Convert.ToString(targetUserId);
        }

        public async Task<string> GetProjectName(int projectId)
        {
            string projectName = await _projectGateway.GetProjectName(projectId);
            return projectName;
        }

        public async Task<string> GetUserName(int userId)
        {
            string userName = await _userGateway.GetPseudoById(userId);
            return userName;
        }
    }
}
