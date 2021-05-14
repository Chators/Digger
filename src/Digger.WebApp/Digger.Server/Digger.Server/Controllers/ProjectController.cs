using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Digger.Server.Authentification;
using Digger.Server.DGraph;
using Digger.Server.Hubs;
using Digger.Server.Models;
using Digger.Server.Models.Project;
using DiStock.DAL;
using DiStock.DAL.Datas;
using DiStock.DAL.Datas.Project;
using DiStock.DAL.Datas.Request;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Digger.Server.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("/api/[controller]")]
    public class ProjectController : Controller
    {
        readonly ProjectGateway _projectGateway;
        readonly UserGateway _userGateway;
        readonly RequestGateway _requestGateway;
        readonly ProjectHub _projectHub;
        readonly DGraphGateway _dGraphGateway;
        readonly GetAccessUser _getAccessUser;

        public ProjectController(ProjectGateway projectGateway, UserGateway userGateway, RequestGateway requestGateway, ProjectHub projectHub, DGraphGateway dGraphGateway, GetAccessUser getAccessUser)
        {
            _projectGateway = projectGateway;
            _userGateway = userGateway;
            _requestGateway = requestGateway;
            _projectHub = projectHub;
            _dGraphGateway = dGraphGateway;
            _getAccessUser = getAccessUser;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetAllProject")]
        public async Task<IActionResult> GetAllProject()
        {
            IEnumerable<ProjectData> result = await _projectGateway.GetAllProject();
            if (result == null) return BadRequest("No project exists");

            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetUserInProject/{projectId}")]
        public async Task<IActionResult> GetUserInProject(int projectId)
        {
            EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), projectId);
            if (!HttpContext.User.IsInRole("admin") && projectAccessRight == EnumProjectAccessRight.None) return StatusCode(403, "Access Denied !");

            IEnumerable<UserInProjectData> result = await _projectGateway.GetUserInProject(projectId);
            if (result == null) return BadRequest("Project has no user");

            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("GetInfoFormAccessRight")]
        public async Task<IActionResult> GetInfoFormAccessRight()
        {
            IEnumerable<ProjectAccessRightData> result = await _projectGateway.GetStaticProjectAccessRight();
            if (result == null) return BadRequest("No access right avaible");

            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetProjectById/{projectId}")]
        public async Task<IActionResult> GetProjectById(int projectId)
        {
            Result<ProjectData> result = await _projectGateway.GetProjectById(projectId);
            if (result.ErrorMessage == "Project not found") return BadRequest(result.ErrorMessage);

            EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), projectId);
            if (!HttpContext.User.IsInRole("admin") && projectAccessRight == EnumProjectAccessRight.None && result.Content.IsPublic == 0) return StatusCode(403, "Access Denied !");

            return Ok(result.Content);
        }

        [Authorize]
        [HttpGet("GetProjectByUserId/{userId}")]
        public async Task<IActionResult> GetProjectByUserId(int userId)
        {
            if (userId == 0) userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!HttpContext.User.IsInRole("admin") && !_getAccessUser.UserCookieIs(HttpContext, Convert.ToString(userId))) return StatusCode(403, "Access Denied !");

            IEnumerable<ProjectUserIdData> result = await _projectGateway.GetProjectByUserId(userId);
            if (result == null) return BadRequest("User has not project");

            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetProjectPublicByUserId/{userId}")]
        public async Task<IActionResult> GetProjectPublicByUserId(int userId)
        {
            // On récupère uniquement les projets public où l'utilisateur n'est pas membre
            if (userId == 0) userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!HttpContext.User.IsInRole("admin") && !_getAccessUser.UserCookieIs(HttpContext, Convert.ToString(userId))) return StatusCode(403, "Access Denied !");

            IEnumerable<ProjectData> result = await _projectGateway.GetProjectPublicByUserId(userId);
            if (result == null) return BadRequest("No public project for this user");

            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetUserInvitationByUserId/{userId}")]
        public async Task<IActionResult> GetUserInvitationByUserId(int userId)
        {
            if (userId == 0) userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!HttpContext.User.IsInRole("admin") && !_getAccessUser.UserCookieIs(HttpContext, Convert.ToString(userId))) return StatusCode(403, "Access Denied !");

            IEnumerable<ProjectInvitationData> result = await _projectGateway.GetUserInvitationByUserId(userId);
            if (result == null) return BadRequest("User has not invitation project");

            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetAuthorInvitationByUserId/{userId}")]
        public async Task<IActionResult> GetAuthorInvitationByUserId(int userId)
        {
            if (userId == 0) userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!HttpContext.User.IsInRole("admin") && !_getAccessUser.UserCookieIs(HttpContext, Convert.ToString(userId))) return StatusCode(403, "Access Denied !");

            IEnumerable<ProjectInvitationData> result = await _projectGateway.GetAuthorInvitationByUserId(userId);
            if (result == null) return BadRequest("User has not send invitation to project");

            return Ok(result);
        }

        [Authorize]
        [HttpPost("CreateProject")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectViewModel model)
        {
            if (model.FktUser == 0) model.FktUser = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!HttpContext.User.IsInRole("admin") && !_getAccessUser.UserCookieIs(HttpContext, model.FktUser.ToString())) return StatusCode(403, "Access Denied !");

            Result<int> result = await _projectGateway.CreateProject(model.FktUser, model.Name, model.Description, 3, model.IsPublic);
            if (result.ErrorMessage == "Project with this name already exists") return BadRequest(result.ErrorMessage);

            return Ok("Project has been created");
        }

        [Authorize]
        [HttpPut("UpdateProject")]
        public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectViewModel model)
        {
            EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), model.Id);
            if (!HttpContext.User.IsInRole("admin") && projectAccessRight != EnumProjectAccessRight.Admin) return StatusCode(403, "Access Denied !");

            Result result = await _projectGateway.UpdateProject(model.Id, model.Name, model.Description, model.IsPublic);
            if (result.ErrorMessage == "Project with this id not exists") return BadRequest(result.ErrorMessage);
            if (result.ErrorMessage == "Project with this name already exists") return BadRequest(result.ErrorMessage);

            return Ok("Project has been updated");
        }

        [Authorize]
        [HttpPut("ChangeAccessRightUserInProject")]
        public async Task<IActionResult> ChangeAccessRightUserInProject([FromBody] ChangeAccessRightUserInProjectViewModel model)
        {
            EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), model.ProjectId);
            if (!HttpContext.User.IsInRole("admin") && projectAccessRight != EnumProjectAccessRight.Admin) return StatusCode(403, "Access Denied !");

            if (Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)) == model.UserId) return BadRequest("You cannot change your access right in project");

            Result result = await _projectGateway.UpdateAccessRightUserInProject(model.UserId, model.ProjectId, model.AccessRightId);
            if (result.ErrorMessage == "User not belongs in this project") return BadRequest(result.ErrorMessage);

            return Ok("Access right changed");
        }

        [Authorize]
        [HttpDelete("DeleteProject/{projectId}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), projectId);
            if (!HttpContext.User.IsInRole("admin") && projectAccessRight != EnumProjectAccessRight.Admin) return StatusCode(403, "Access Denied !");

            _dGraphGateway.DeleteProject(projectId);
            Result result = await _projectGateway.DeleteProject(projectId);
            if (result.ErrorMessage == "Project not found") return BadRequest(result.ErrorMessage);

            return Ok("Project has been deleted");
        }
        
        [Authorize]
        [HttpPost("SendInvitationProject")]
        public async Task<IActionResult> SendInvitationProject([FromBody] SendInvitationProjectViewModel model)
        {
            if (model.UserAuthorId == 0) model.UserAuthorId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (model.UserAuthorId == model.UserInvitedId) return BadRequest("User author cannot invite himself");
            int userInProject = await _projectGateway.CheckUserInProject(model.UserInvitedId, model.ProjectId);
            if (userInProject != 0) return BadRequest("User invited already member of this project");

            if (!HttpContext.User.IsInRole("admin"))
            {
                if(!_getAccessUser.UserCookieIs(HttpContext, model.UserAuthorId.ToString())) return StatusCode(403, "Access Denied !");
                EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), model.ProjectId);
                if (projectAccessRight != EnumProjectAccessRight.Admin) return StatusCode(403, "Access Denied !");
            }

            Result resultSendInvitationProject = await _projectGateway.CreateUserInvitationInProject(model.UserAuthorId, model.UserInvitedId, model.ProjectId);
            if (resultSendInvitationProject.ErrorMessage == "An invitation made by the user with this project and user invited already exist") return BadRequest(resultSendInvitationProject.ErrorMessage);

            string nameProject = await _getAccessUser.GetProjectName(model.ProjectId);
            await _projectHub.ReceiveInvitationInProject(Convert.ToString(model.UserInvitedId), Convert.ToString(model.UserAuthorId), Convert.ToString(model.ProjectId), HttpContext.User.Identity.Name, nameProject);

            return Ok("The invitation has been sent");
        }

        [Authorize]
        [HttpPost("AssignUserToProject")]
        public async Task<IActionResult> AssignUserToProject([FromBody] AssignUserToProject model)
        {
            if (model.UserId == 0) model.UserId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!HttpContext.User.IsInRole("admin") && !_getAccessUser.UserCookieIs(HttpContext, model.UserId.ToString())) return StatusCode(403, "Access Denied !");

            int userInvitationId = await _projectGateway.CheckUserInvitation(model.UserId, model.ProjectId);
            if (userInvitationId == 0) return BadRequest("Invitation not exists");

            Result resultAssignProject = await _projectGateway.AssignUserToProject(model.UserId, model.ProjectId, (int)EnumProjectAccessRight.Voyeur);
            if (resultAssignProject.ErrorMessage == "The project is already assigned to user") return BadRequest(resultAssignProject.ErrorMessage);

            Result resultDeleteInvitationProject = await _projectGateway.DeleteUserInvitationInProjectByUserInvitedAndProject(model.UserId, model.ProjectId);

            string nameProject = await _getAccessUser.GetProjectName(model.ProjectId);
            string userName = await _getAccessUser.GetUserName(model.UserId);
            IEnumerable<string> userInProject = await _projectGateway.GetIdUserInProject(model.ProjectId);
            await _projectHub.ReceiveUserJoinedProject(userInProject.ToList(), Convert.ToString(model.ProjectId), userName, nameProject);

            return Ok("Assign project success");
        }

        [Authorize]
        [HttpDelete("UnassignUserToProject")]
        public async Task<IActionResult> UnassignUserToProject([FromBody] UnassignUserToProjectViewModel model)
        {
            if (model.UserId == 0) model.UserId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), model.ProjectId);
            if (!HttpContext.User.IsInRole("admin") && !_getAccessUser.UserCookieIs(HttpContext, model.UserId.ToString()) && projectAccessRight != EnumProjectAccessRight.Admin) return StatusCode(403, "Access Denied !");

            Result resultUnassignUserToProject = await _projectGateway.UnassignUserToProject(model.UserId, model.ProjectId);
            if (resultUnassignUserToProject.ErrorMessage == "User is not in the project") return BadRequest(resultUnassignUserToProject.ErrorMessage);

            int nbOfAdminInProject = await _projectGateway.GetNumberAccessRightInProject(model.ProjectId, (int)EnumProjectAccessRight.Admin);
            if (nbOfAdminInProject <= 0)
            {
                _dGraphGateway.DeleteProject(model.ProjectId);
                await _projectGateway.DeleteProject(model.ProjectId);
            }

            string nameProject = await _getAccessUser.GetProjectName(model.ProjectId);
            string userName = await _getAccessUser.GetUserName(model.UserId);
            IEnumerable<string> userInProject = await _projectGateway.GetIdUserInProject(model.ProjectId);
            await _projectHub.ReceiveUserLeavedProject(userInProject.ToList(), Convert.ToString(model.ProjectId), userName, nameProject);

            return Ok("Unassign success");
        }

        [Authorize]
        [HttpDelete("CancelInvitationProject")]
        public async Task<IActionResult> CancelInvitationProject([FromBody] CancelInvitationProjectViewModel model)
        {
            if (model.UserInvitedId == 0) model.UserInvitedId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (model.UserAuthorId == 0) model.UserAuthorId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!HttpContext.User.IsInRole("admin") && !_getAccessUser.UserCookieIs(HttpContext, model.UserAuthorId.ToString()) && !_getAccessUser.UserCookieIs(HttpContext, model.UserInvitedId.ToString())) return StatusCode(403, "Access Denied !");

            Result resultDeleteInvitationProject = await _projectGateway.DeleteUserInvitationInProject(model.UserAuthorId, model.UserInvitedId, model.ProjectId);
            if (resultDeleteInvitationProject.ErrorMessage == "No invitation with this user author id, user invited id and project id exists") return BadRequest(resultDeleteInvitationProject.ErrorMessage);

            return Ok("Cancel invitation success");
        }
    }
}