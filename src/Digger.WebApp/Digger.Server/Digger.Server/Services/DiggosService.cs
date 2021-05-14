using Digger.Server.Models;
using Digger.Server.Models.Request;
using Digger.Server.Models.Software;
using Digger.Server.Tools;
using DiStock.DAL;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Digger.Server.Services
{
    public class DiggosService
    {
        readonly RequestGateway _requestGateway;
        readonly IOptions<DiggosServiceOptions> _options;
        readonly TokenService _tokenService;

        public DiggosService(IOptions<DiggosServiceOptions> options, RequestGateway requestGateway, TokenService tokenService)
        {
            _options = options;
            _requestGateway = requestGateway;
            _tokenService = tokenService;
        }

        public async Task<HttpResponseMessage> RunSoftware( StartRequestViewModel model, int requestId )
        {
            string url = _options.Value.Url + "Software/Search";
            string jsonModel = Json.Serialize<StartRequestViewModel>(model);

            using (HttpClient client = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url)))
            {
                request.Content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", string.Format("Bearer {0}", _tokenService.GenerateToken()));
                
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    return response;
                }
            }
        }

        public async Task<HttpResponseMessage> InstallSoftware ( SoftwareForDiggosViewModel model )
        {
            string url = _options.Value.Url + "Software/Install";
            string jsonModel = Json.Serialize<SoftwareForDiggosViewModel>(model);

            using (HttpClient client = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url)))
            {
                request.Content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", string.Format("Bearer {0}", _tokenService.GenerateToken()));
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    return response;
                }
            }
        }

        public async Task<HttpResponseMessage> UninstallSoftware(int id)
        {
            string url = _options.Value.Url + "Software/Uninstall/" + id;

            using (HttpClient client = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, new Uri(url)))
            {
                request.Headers.Add("Authorization", string.Format("Bearer {0}", _tokenService.GenerateToken()));
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    return response;
                }
            }
        }

        public async Task<string> GetListNameFileInDocker(string imageDockerName)
        {
            string url = _options.Value.Url + "Software/GetListNameFileInDocker/" + imageDockerName;

            using (HttpClient client = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(url)))
            {
                request.Headers.Add("Authorization", string.Format("Bearer {0}", _tokenService.GenerateToken()));
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
        }
    }

    public class DiggosServiceOptions
    {
        public string Url { get; set; }
    }
}
