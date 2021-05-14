using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Digger.Server.Authentification;
using Digger.Server.Models.Project;
using Digger.Server.Models.Request;
using Digger.Server.Services;
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
    public class RequestController : Controller
    {
        readonly ProjectGateway _projectGateway;
        readonly RequestGateway _requestGateway;
        readonly DiggosService _diggosService;
        readonly GetAccessUser _getAccessUser;

        public RequestController(ProjectGateway projectGateway, RequestGateway requestGateway, DiggosService diggosService, GetAccessUser getAccessUser)
        {
            _projectGateway = projectGateway;
            _requestGateway = requestGateway;
            _diggosService = diggosService;
            _getAccessUser = getAccessUser;
        }

        [Authorize]
        [HttpGet("GetRequestById/{requestId}")]
        public async Task<IActionResult> GetRequestById(int requestId)
        {
            RequestData requestResult = await _requestGateway.GetRequestById(requestId);
            if (requestResult == null) return BadRequest("Request not found");

            if (!HttpContext.User.IsInRole("admin"))
            {
                EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), requestResult.ProjectId);
                if (projectAccessRight == EnumProjectAccessRight.None)
                {
                    ProjectIsPublic projectIsPublic = await _projectGateway.ProjectIsPublic(requestResult.ProjectId);
                    if (projectIsPublic.IsPublic == 1) return StatusCode(403, "Access Denied !");
                }
            }

            return Ok(requestResult);
        }

        [Authorize]
        [HttpGet("GetTypeEntity")]
        public async Task<IActionResult> GetTypeEntity()
        {
            IEnumerable<string>aTypeEntity = await _requestGateway.GetTypeEntity();
            if (aTypeEntity == null) return BadRequest("No type entity found");

            return Ok(aTypeEntity);
        }

        [Authorize]
        [HttpGet("GetRequestByProjectId/{projectId}")]
        public async Task<IActionResult> GetRequestByProjectId(int projectId)
        {
            if (!HttpContext.User.IsInRole("admin"))
            {
                EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), projectId);
                if (projectAccessRight == EnumProjectAccessRight.None)
                {
                    ProjectIsPublic projectIsPublic = await _projectGateway.ProjectIsPublic(projectId);
                    if (projectIsPublic.IsPublic == 1) return StatusCode(403, "Access Denied !");
                }
            }

            IEnumerable<RequestByProjectIdData> requestResult = await _requestGateway.GetRequestByProjectId(projectId);
            if (requestResult == null) return BadRequest("No request for this project id exists");

            return Ok(requestResult);
        }

        [Authorize]
        [HttpPost("StartRequest/{projectId}")]
        public async Task<IActionResult> StartRequest(int projectId, [FromBody] StartRequestViewModel model)
        {
            if (model.Softwares.Count <= 0) return BadRequest("Software needed");

            if (!HttpContext.User.IsInRole("admin"))
            {
                EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), projectId);
                if (projectAccessRight != EnumProjectAccessRight.Worker || projectAccessRight != EnumProjectAccessRight.Admin) return StatusCode(403, "Access Denied !");
            }

            Result<int> createRequest = await _requestGateway.CreateRequest(1, projectId, model.DataEntity, model.UidNode, HttpContext.User.Identity.Name);
            if (createRequest.ErrorMessage == "Already research with this date exists") return BadRequest(createRequest.ErrorMessage);

            model.RequestId = createRequest.Content;
            HttpResponseMessage response = await _diggosService.RunSoftware(model, createRequest.Content);
            if (!response.IsSuccessStatusCode)
            {
                await _requestGateway.ChangeStatusRequest(createRequest.Content, 5);
                return StatusCode(502, "Error on diggos");
            }

            Result<bool> statusRequestSuccess = await _requestGateway.ChangeStatusRequest(createRequest.Content, 2);

            return Ok("Request started");
        }

        [Authorize]
        [HttpDelete("DeleteRequest/{requestId}")]
        public async Task<IActionResult> DeleteRequest(int requestId)
        {
            RequestData requestResult = await _requestGateway.GetRequestById(requestId);
            if (requestResult == null) return BadRequest("Request not found");

            if (!HttpContext.User.IsInRole("admin"))
            {
                EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), requestResult.ProjectId);
                if (projectAccessRight != EnumProjectAccessRight.Worker || projectAccessRight != EnumProjectAccessRight.Admin) return StatusCode(403, "Access Denied !");
            }

            Result<bool> result = await _requestGateway.DeleteRequest(requestId);
            if (result.ErrorMessage == "Request not found") return BadRequest(result.ErrorMessage);

            return Ok("Request deleted");
        }
    }
}