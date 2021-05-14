using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Digger.Server.Tests.Helpers
{
    public static class GetAutorizationDigger
    {
        static string _password = "lol";

        public static async Task<HttpClient> GetCookieAdmin(TestServer testServer, string pseudo)
        {
            /*CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;*/

            //HttpClient client = new HttpClient(handler);
            HttpClientHandler handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;
            handler.UseCookies = true;
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = testServer.BaseAddress;
            BddDiStock.CreateUser(pseudo, "admin");
            HttpContent content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "Pseudo", pseudo },
                { "Password", _password }
            });
            var response = await client.PostAsync("/Login", content);
            return client;
        }

        public static async Task<string> GetCookieUser(HttpClient client, string pseudo)
        {
            BddDiStock.CreateUser(pseudo, "user");

            HttpContent content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                        { "Pseudo", pseudo },
                        { "Password", _password },
                        { "ConfirmPassword", _password }
                });
            var response = await client.PostAsync("/Login", content);

            return "";
        }
    }
}
