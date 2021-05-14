using Digger.Server.Tests.Helpers;
using Digger.Server.Tools;
using DiStock.DAL;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Digger.Server.Tests
{
    [TestFixture]
    public class UserTests
    {

        #region Register
        [Test]
        public async Task register_good()
        {
            using (TestServer server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<Startup>()))
            using (HttpClient client = server.CreateClient())
            {
                string pseudo = "test99999test";
                string password = "lol";
                string confirmPassword = password;
                HttpContent content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { "Pseudo", pseudo },
                        { "Password", password },
                        { "ConfirmPassword", confirmPassword }
                    });
                var response = await client.PostAsync("/Register", content);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
                Assert.That(response.Headers.GetValues("Location").Any(x => x == "/Home"));
                BddDiStock.DeleteUser("test99999test");
            }
        }
        [Test]
        public async Task register_with_pseudo_already_used()
        {
            using (TestServer server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<Startup>()))
            using (HttpClient client = server.CreateClient())
            {
                string pseudo = "test99999test";
                string password = "ll";
                string confirmPassword = password;
                HttpContent content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { "Pseudo", pseudo },
                        { "Password", password },
                        { "ConfirmPassword", confirmPassword }
                    });
                var response = await client.PostAsync("/Register", content);

                pseudo = "test99999test";
                password = "ll";
                confirmPassword = password;
                content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { "Pseudo", pseudo },
                        { "Password", password },
                        { "ConfirmPassword", confirmPassword }
                    });
                response = await client.PostAsync("/Register", content);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("Le Pseudo est déjà utilisé !"));

                BddDiStock.DeleteUser("test99999test");
            }
        }

        [Test]
        public async Task register_bad_confirm_password()
        {
            using (TestServer server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<Startup>()))
            using (HttpClient client = server.CreateClient())
            {
                string pseudo = "test99999test";
                string password = "titi";
                string confirmPassword = "azozo";
                HttpContent content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { "Pseudo", pseudo },
                        { "Password", password },
                        { "ConfirmPassword", confirmPassword }
                    });
                var response = await client.PostAsync("/Register", content);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("Les 2 mots de passe ne sont pas identiques !"));
            }
        }
        #endregion

        #region Login

        #endregion

        #region Logout

        #endregion

        #region GetAllUser
        [Test]
        public async Task get_all_users_in_admin()
        {
            string userName = RandomString();
            string testUser1 = RandomString();
            string testUser2 = RandomString();
            using (TestServer server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<Startup>()))
            using (HttpClient client = await GetAutorizationDigger.GetCookieAdmin(server, userName))
            {
                //HttpClient client = server.CreateClient();

                BddDiStock.CreateUser(testUser1, "user");
                BddDiStock.CreateUser(testUser2, "user");


                using (HttpResponseMessage response = await client.GetAsync("/api/User/GetAllUsers"))
                using (StreamReader sr = new StreamReader(await response.Content.ReadAsStreamAsync()))
                using (JsonTextReader jsonReader = new JsonTextReader(sr))
                {
                    JArray json = await JArray.LoadAsync(jsonReader);
                    List<UserData> s = json.Select(u =>
                    {
                        JObject user = (JObject)u;
                        return new UserData
                        {
                            Id = ((JProperty)user["id"]).Value<int>(),
                            Pseudo = ((JProperty)user["pseudo"]).Value<string>(),
                            Role = ((JProperty)user["role"]).Value<string>()
                        };
                    }).ToList();

                    //Assert.That(s.FindAll(x => x.Pseudo));
                    //Console.WriteLine(s);
                    //Assert.That(response.Content.ReadAsStringAsync);

                    BddDiStock.DeleteUser(testUser1);
                    BddDiStock.DeleteUser(testUser2);
                }
            }
        }

        [Test]
        public async Task get_all_users_in_user()
        {
            throw new NotImplementedException();
            using (TestServer server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<Startup>()))
            {
                HttpClient client = server.CreateClient();

                BddDiStock.CreateUser("test99999test", "user");
                BddDiStock.CreateUser("test99998test", "user");


                var response = await client.GetAsync("/api/User/GetAllUsers");

                BddDiStock.DeleteUser("test99999test");
                BddDiStock.DeleteUser("test99998test");
            }
        }

        [Test]
        public async Task get_all_users_in_anonymous()
        {
            throw new NotImplementedException();
            using (TestServer server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<Startup>()))
            {
                HttpClient client = server.CreateClient();

                BddDiStock.CreateUser("test99999", "user");
                BddDiStock.CreateUser("test99998", "user");


                var response = await client.GetAsync("/api/User/GetAllUsers");

                BddDiStock.DeleteUser("test99999test");
                BddDiStock.DeleteUser("test99998test");
            }
        }
        #endregion
        /*getuserbypseudo
        deleteuser
        updateuser
        assignproject
        unassignproject
        */

        string RandomString() => string.Format("Test-{0}", Guid.NewGuid().ToString().Substring(0, 8));
    }
}
