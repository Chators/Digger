using Digger.Server.Authentification;
using Digger.Server.DGraph;
using Digger.Server.Models.Graph;
using Digger.Server.Models.Project;
using DiStock.DAL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Digger.Server.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("/api/[controller]")]
    public class GraphController : Controller
    {
        readonly ProjectGateway _projectGateway;
        readonly DGraphGateway _dGraphGateway;
        readonly GetAccessUser _getAccessUser;

        public GraphController(ProjectGateway projectGateway, DGraphGateway dGraphGateway, GetAccessUser getAccessUser)
        {
            _projectGateway = projectGateway;
            _dGraphGateway = dGraphGateway;
            _getAccessUser = getAccessUser;
        }

        [Authorize]
        [HttpGet("GetProjectGraphById/{projectId}")]
        public async Task<IActionResult> GetProjectGraphById(int projectId)
        {
            Result<ProjectData> result = await _projectGateway.GetProjectById(projectId);
            if (result.ErrorMessage == "Project not found") return BadRequest(result.ErrorMessage);

            bool userCanReadProject = await UserCanReadProject(projectId, result.Content);
            if (!userCanReadProject) return StatusCode(403, "Access Denied !");

            FluentResults.Result<string> resultGraphByProjectId = await _dGraphGateway.FindByProjectId(projectId);

            return Ok(resultGraphByProjectId.Value);
        }

        [Authorize]
        [HttpPost("AddNode/{projectId}")]
        public async Task<IActionResult> AddNode(int projectId, [FromBody] AddNodeViewModel model)
        {
            if (String.IsNullOrEmpty(model.Data) && model.Data != null) BadRequest("Manque data");
            if (String.IsNullOrEmpty(model.TypeOfData) && model.TypeOfData != null) BadRequest("Manque type de donée");

            bool userCanModifyProject = await UserCanModifyProject(projectId);
            if (!userCanModifyProject) return StatusCode(403, "Access Denied !");

            string r = _dGraphGateway.GetNodeBy(projectId, model.Data, model.TypeOfData);
            dynamic nodeDGraph = JsonConvert.DeserializeObject(r);

            bool nodeExists = nodeDGraph.FindNode.Count > 0;
            if (nodeExists) return BadRequest("Node already exists");

            string jsonNodeCreated = await _dGraphGateway.AddNode(projectId, HttpContext.User.Identity.Name, model.Data, model.Note, model.Source, model.TypeOfData);

            // HUB PROJECT ACTU

            return Ok(jsonNodeCreated);
        }

        [Authorize]
        [HttpPost("AddEdge/{projectId}")]
        public async Task<IActionResult> AddEdge(int projectId, [FromBody] AddEdgeViewModel model)
        {
            bool userCanModifyProject = await UserCanModifyProject(projectId);
            if (!userCanModifyProject) return StatusCode(403, "Access Denied !");

            await _dGraphGateway.AddEdge(projectId, model.SourceNode, model.TargetNode);

            // HUB PROJECT ACTU

            return Ok("Edge added");
        }

        [Authorize]
        [HttpPut("ModifyNode/{projectId}")]
        public async Task<IActionResult> ModifyNode(int projectId, [FromBody] ModifyNodeViewModel model)
        {
            if (String.IsNullOrEmpty(model.Data) && model.Data != null) BadRequest("Manque data");
            if (String.IsNullOrEmpty(model.TypeOfData) && model.TypeOfData != null) BadRequest("Manque type de donée");

            bool userCanModifyProject = await UserCanModifyProject(projectId);
            if (!userCanModifyProject) return StatusCode(403, "Access Denied !");

            string r = _dGraphGateway.GetNodeBy(projectId, model.Data, model.TypeOfData);
            dynamic nodeDGraph = JsonConvert.DeserializeObject(r);

            bool nodeExists = nodeDGraph.FindNode.Count > 0;
            if (nodeExists) return BadRequest("Node already exists");

            await _dGraphGateway.ModifyNode(projectId, model.Uid, HttpContext.User.Identity.Name, model.Data, model.Note, model.Source, model.TypeOfData);

            // HUB PROJECT ACTU

            return Ok("Node updated");
        }

        [Authorize]
        [HttpDelete("DeleteNodes/{projectId}")]
        public async Task<IActionResult> DeleteNodes(int projectId, [FromBody] DeleteNodesViewModel model)
        {
            bool userCanModifyProject = await UserCanModifyProject(projectId);
            if (!userCanModifyProject) return StatusCode(403, "Access Denied !");

            await _dGraphGateway.DeleteNodesById(projectId, model.NodesId);

            // HUB PROJECT ACTU

            return Ok("Node(s) deleted");
        }

        [Authorize]
        [HttpDelete("DeleteEdge/{projectId}")]
        public async Task<IActionResult> DeleteEdge(int projectId, [FromBody] DeleteEdgeViewModel model)
        {
            bool userCanModifyProject = await UserCanModifyProject(projectId);
            if (!userCanModifyProject) return StatusCode(403, "Access Denied !");
            
            await _dGraphGateway.DeleteEdge(projectId, model.SourceNode, model.TargetNode);

            // HUB PROJECT ACTU

            return Ok("Edge deleted");
        }
        
        private async Task<bool> UserCanReadProject(int projectId, ProjectData project)
        {
            EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), projectId);
            if (!HttpContext.User.IsInRole("admin") && projectAccessRight == EnumProjectAccessRight.None && project.IsPublic == 0) return false;
            return true;
        }

        private async Task<bool> UserCanModifyProject(int projectId)
        {
            EnumProjectAccessRight projectAccessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), projectId);
            if (!HttpContext.User.IsInRole("admin") && projectAccessRight != EnumProjectAccessRight.Admin && projectAccessRight != EnumProjectAccessRight.Worker) return false;
            return true;
        }
    }
}