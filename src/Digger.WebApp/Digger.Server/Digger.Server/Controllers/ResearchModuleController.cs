using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Digger.Server.Models;
using Digger.Server.Models.ResearchModule;
using Digger.Server.Services;
using DiStock.DAL;
using DiStock.DAL.Datas;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Digger.Server.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("/api/[controller]")]
    public class ResearchModuleController : Controller
    {
        readonly SoftwareGateway _softwareGateway;
        readonly ResearchModuleGateway _researchModuleGateway;
        readonly DiggosService _diggosService;

        public ResearchModuleController(SoftwareGateway softwareGateway, ResearchModuleGateway researchModuleGateway, DiggosService diggosService)
        {
            _softwareGateway = softwareGateway;
            _researchModuleGateway = researchModuleGateway;
            _diggosService = diggosService;
        }

        [Authorize]
        [HttpGet("GetResearchModuleById/{researchModuleId}")]
        public async Task<IActionResult> GetResearchModuleById(int researchModuleId)
        {
            Result<ResearchModuleData> result = await _researchModuleGateway.GetResearchModuleById(researchModuleId);
            if (result.Content == null) return BadRequest("Research Module not found");

            return Ok(result.Content);
        }

        [Authorize]
        [HttpGet("GetInfoFormResearchModule/{idSoftware}")]
        public async Task<IActionResult> GetInfoFormResearchModule(int idSoftware)
        {
            Result<InfoFormResearchModuleData> result = await _researchModuleGateway.GetInfoFormResearchModule();
            List<string> nameFileOnDiggos = await GetNameFileOsintOnDiggos(Convert.ToString(idSoftware));
            IEnumerable<ResearchModuleData> ieResearchModuleInstalled = await _researchModuleGateway.GetResearchModuleBySoftwareId(idSoftware);
            
            if (ieResearchModuleInstalled != null)
            {
                List<ResearchModuleData> researchModuleInstall = ieResearchModuleInstalled.ToList<ResearchModuleData>();
                for (int i = 0; i < nameFileOnDiggos.Count; i++)
                {
                    string nameFile = nameFileOnDiggos[i];
                    foreach(ResearchModuleData researchModule in researchModuleInstall)
                    {
                        if (nameFile == researchModule.Name)
                        {
                            nameFileOnDiggos.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            result.Content.NameFileOsintAvaibles = nameFileOnDiggos;

            return Ok(result.Content);
        }

        [Authorize]
        [HttpGet("GetSoftwareModuleResearchByTypeEntity/{typeEntity}")]
        public async Task<IActionResult> GetSoftwareModuleResearchByTypeEntity(string typeEntity)
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

            for (int i = 0; i < _softwaresAndResearch.Count; i++)
            {
                SoftwareAndResearchData softwareResearchModule = _softwaresAndResearch[i];
                for (int y = 0; y < softwareResearchModule.ResearchModule.Count; y++)
                {
                    ResearchModuleData researchModules = softwareResearchModule.ResearchModule[y];
                    if (researchModules.TypeEntity != typeEntity)
                    {
                        softwareResearchModule.ResearchModule.RemoveAt(y);
                        y--;
                    }
                }
                if (softwareResearchModule.ResearchModule.Count <= 0)
                {
                    _softwaresAndResearch.RemoveAt(i);
                    i--;
                }
            }
            if (_softwaresAndResearch == null) return BadRequest("Research module with this type entity not found");

            return Ok(_softwaresAndResearch);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UploadResearchModule")]
        public async Task<IActionResult> UploadResearchModule([FromBody] UploadResearchModuleViewModel model)
        {
            Result<SoftwareData> r = await _softwareGateway.GetSoftwareById(model.FktSoftwareOnDiggos);
            if (r.Content == null) BadRequest("Software not found");

            List<string> listOsintFile = await GetNameFileOsintOnDiggos(Convert.ToString(model.FktSoftwareOnDiggos));
            bool find = false;
            foreach (string file in listOsintFile)
            {
                if (file == model.Name)
                {
                    find = true;
                    break;
                }
            }
            if (!find) return BadRequest("Research Module not found");

            Result<int> result = await _researchModuleGateway.CreateResearchModule(model.FktSoftwareOnDiggos, model.FktStaticEntity, model.FktStaticFootprint, model.Name, model.Description);
            if (result.ErrorMessage == "Research Module already exists") return BadRequest("Research Module already exists");

            return Ok(result.Content);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("AddTypeEntity/{typeEntity}")]
        public async Task<IActionResult> AddTypeEntity(string typeEntity)
        {
            Result result = await _researchModuleGateway.AddTypeEntity(typeEntity);
            if (result.ErrorMessage == "Type Entity already exists") return BadRequest(result.ErrorMessage);
            
            return Ok("Type Entity created");
        }

        [Authorize(Roles = "admin")]
        [HttpPut("UpdateResearchModule")]
        public async Task<IActionResult> UpdateResearchModule([FromBody] UpdateResearchModuleViewModel model)
        {
            Result result = await _researchModuleGateway.UpdateResearchModuleById(model.Id, model.FktStaticEntity, model.FktStaticFootprint, model.Name, model.Description);
            if (result.ErrorMessage == "Research Module not found") return BadRequest(result.ErrorMessage);
            if (result.ErrorMessage == "Research Module with this name already exists") return BadRequest(result.ErrorMessage);

            return Ok("Research Module has been updated");
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteResearchModule/{researchModuleId}")]
        public async Task<IActionResult> DeleteResearchModule(int researchModuleId)
        {
            Result result = await _researchModuleGateway.DeleteResearchModuleById(researchModuleId);
            if (result.ErrorMessage == "Research Module not found") return BadRequest(result.ErrorMessage);

            return Ok("Research Module has been deleted");
        }

        private async Task<List<string>> GetNameFileOsintOnDiggos(string nameSoftware)
        {
            string nameOsintSoftwares = await _diggosService.GetListNameFileInDocker(nameSoftware);

            string delimiter = "\n";
            string[] nameOsintSofts = nameOsintSoftwares.Split(delimiter);
            List<string> r = new List<string>();

            for (int i = 0; i < nameOsintSofts.Length - 1; i++)
            {
                string nameFile = nameOsintSofts[i];
                r.Add(nameFile);
            }

            if (nameOsintSofts[nameOsintSofts.Length - 1] == "") return r;
            else
            {
                r.Add(nameOsintSofts[nameOsintSofts.Length]);
                return r;
            }
        }
    }
}