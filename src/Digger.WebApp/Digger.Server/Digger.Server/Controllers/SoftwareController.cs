using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper;
using Digger.Server.Models;
using Digger.Server.Models.Software;
using Digger.Server.Services;
using DiStock.DAL;
using DiStock.DAL.Datas;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Digger.Server.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("/api/[controller]")]
    public class SoftwareController : Controller
    {
        readonly SoftwareGateway _softwareGateway;
        readonly ResearchModuleGateway _researchModuleGateway;
        readonly DiggosService _diggosService;

        public SoftwareController(SoftwareGateway softwareGateway, ResearchModuleGateway researchModuleGateway, DiggosService diggosService)
        {
            _softwareGateway = softwareGateway;
            _researchModuleGateway = researchModuleGateway;
            _diggosService = diggosService;
        }

        [Authorize]
        [HttpGet("GetAllSoftwareAndResearch")]
        public async Task<IActionResult> GetAllSoftwareAndResearch()
        {
            IEnumerable<SoftwareData> _ieSoftwares = await _softwareGateway.GetAllSoftware();
            if (_ieSoftwares == null) return BadRequest("Software not found");

            List<SoftwareData> _softwares = _ieSoftwares.AsList();
            List<SoftwareAndResearchData> _softwaresAndResearch = new List<SoftwareAndResearchData>();
            for (int i = 0; i < _softwares.Count; i++)
            {
                IEnumerable<ResearchModuleData> researchModules = await _researchModuleGateway.GetResearchModuleBySoftwareId(_softwares[i].Id);
                _softwaresAndResearch.Add(new SoftwareAndResearchData(_softwares[i], researchModules.AsList()));
            }

            return Ok(_softwaresAndResearch);
        }

        [HttpGet("GetSoftwareById/{idSoftware}")]
        public async Task<IActionResult> GetSoftwareById(int idSoftware)
        {
            Result<SoftwareData> result = await _softwareGateway.GetSoftwareById(idSoftware);
            if (result.Content == null) return BadRequest("Software not found");

            return Ok(result.Content);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UploadSoftware")]
        public async Task<IActionResult> UploadSoftware([FromBody] SoftwareCreateViewModel model)
        {
            // Lock in bdd
            Result<int> result = await _softwareGateway.CreateSoftware(model.Name, model.Description);
            if (result.ErrorMessage == "Software with this name exists") return BadRequest(result.ErrorMessage);

            SoftwareForDiggosViewModel contentForDiggos = new SoftwareForDiggosViewModel();
            contentForDiggos.Id = result.Content;
            contentForDiggos.LinkProject = model.LinkProject;

            // Upload software on diggos
            HttpResponseMessage theLastResponse = await _diggosService.InstallSoftware(contentForDiggos);
            if (!theLastResponse.IsSuccessStatusCode)
            {
                // Delete in bdd
                await _softwareGateway.DeleteSoftware(result.Content);
                return StatusCode(502, "Error on diggos");
            }
            
            return Ok(result.Content);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteSoftware/{softwareId}")]
        public async Task<IActionResult> UninstallSoftware(int softwareId)
        {
            Result result = await _softwareGateway.DeleteSoftware(softwareId);
            await _diggosService.UninstallSoftware(softwareId);

            return Ok("Software has been uninstalled");
        }

        [Authorize(Roles = "admin")]
        [HttpPut("UpdateSoftware")]
        public async Task<IActionResult> UpdateSoftware([FromBody] SoftwareUpdateViewModel model)
        {
            Result result = await _softwareGateway.UpdateSoftware(model.Id, model.NewName, model.Description);
            if (result.ErrorMessage == "Software not found") return BadRequest(result.ErrorMessage);
            if (result.ErrorMessage == "Software with this name already exists") return BadRequest(result.ErrorMessage);

            return Ok("Software updated");
            
        }
    }
}