using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Digger.Server.Helpers;
using Digger.Server.Models.Authentification;
using DiStock.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Digger.Server.Controllers
{
    [Authorize]
    public class AuthentificationController : Controller
    {
        readonly AuthentificationManager _authentificationManager;
        readonly PasswordHasher _passwordHasher;
        readonly UserGateway _userGateway;

        public AuthentificationController(AuthentificationManager authentificationManager, PasswordHasher passwordHasher, UserGateway userGateway)
        {
            _authentificationManager = authentificationManager;
            _passwordHasher = passwordHasher;
            _userGateway = userGateway;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return RedirectToRoute("spa-fallback");
            return View("Login");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Result<UserData> user = await _userGateway.GetUserByPseudo(model.Pseudo);
            if (user.HasError) return BadRequest("Your identifiers are incorrect !");

            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user.Content.Password, model.Password);

            if (result != PasswordVerificationResult.Failed)
            {
                await SignIn(Convert.ToString(user.Content.Id), user.Content.Pseudo, user.Content.Role);
                return RedirectToRoute("spa-fallback");
            }
            else return BadRequest("Your identifiers are incorrect!");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
        
        [AllowAnonymous]
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (model.Password != model.ConfirmPassword) return BadRequest("The 2 passwords are not the same");

            byte[] passwordHash = _passwordHasher.HashPassword(model.Password);
            string role = "user";

            Result<int> result = await _userGateway.CreateUser(model.Pseudo, passwordHash, role);
            if (result.ErrorMessage == "This pseudo is already used") return BadRequest(result.ErrorMessage);

            return Ok("User created");
        }

        private async Task SignIn(string id, string pseudo, string role)
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    _authentificationManager.GetClaimsPrincipal(Convert.ToString(id), pseudo, role),
                    _authentificationManager.GetAuthProperties());
        }
    }
}