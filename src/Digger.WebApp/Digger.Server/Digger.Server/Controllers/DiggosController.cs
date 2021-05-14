using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGraph.DAL;
using Digger.Server.Authentification;
using Digger.Server.DGraph;
using Digger.Server.Hubs;
using Digger.Server.Models;
using DiStock.DAL;
using DiStock.DAL.Datas;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Digger.Server.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerAuthentication.AuthenticationType)]
    [Route("/api/[controller]")]
    public class DiggosController : Controller
    {
        readonly ProjectGateway _projectGateway;
        readonly RequestGateway _requestGateway;
        readonly DGraphGateway _dGraphGateway;
        readonly ProjectHub _projectHub;
        readonly GraphHub _graphHub;

        public DiggosController(ProjectHub projectHub, RequestGateway requestGateway, ProjectGateway projectGateway, DGraphGateway dGraphGateway, GraphHub graphHub)
        {
            _projectGateway = projectGateway;
            _requestGateway = requestGateway;
            _dGraphGateway = dGraphGateway;
            _projectHub = projectHub;
            _graphHub = graphHub;
        }

        [Authorize]
        [HttpPost("SaveSearchData/{requestId}")]
        public async Task<IActionResult> SaveSearchData(int requestId, [FromBody] StringViewModel resultJson)
        {
            // on change le status request on le met à 3
            await _requestGateway.ChangeStatusRequest(requestId, 3);

            // recup le project Id
            RequestData request = await _requestGateway.GetRequestById(requestId);

            // Verifier que le noeud existe tjrs
            if (_dGraphGateway.NodeExists(request.ProjectId, request.UidNode))
            {
                List<NodeSearchData> nodesSearchData = JsonConvert.DeserializeObject<List<NodeSearchData>>(resultJson.Key);
                List<AddNodesToNodeData> nodes = await _dGraphGateway.CreateAddNodesToNodeData(request.ProjectId, request.Author, request.UidNode, nodesSearchData);
                await _dGraphGateway.AddNodesToNode(request.ProjectId, request.UidNode, nodes);

                _requestGateway.DeleteRequest(requestId);

                FluentResults.Result<string> resultNodes = await _dGraphGateway.FindByProjectId(request.ProjectId);

                // On envoit message à tous les gens du projet
                string nameProject = await GetProjectName(request.ProjectId);
                IEnumerable<string> userInProject = await _projectGateway.GetIdUserInProject(request.ProjectId);
                await _graphHub.ReceiveRequestDoneGiveNewNode(userInProject.ToList(), resultNodes.Value, request.DataEntity);
                await _projectHub.ReceiveRequestDone(userInProject.ToList(), Convert.ToString(request.ProjectId), request.DataEntity, nameProject);
            }

            return Ok("Request done");
        }

        private async Task<string> GetProjectName(int projectId)
        {
            string projectName = await _projectGateway.GetProjectName(projectId);
            return projectName;
        }
    }
}