using Digger.Server.Authentification;
using Digger.Server.Helpers;
using Digger.Server.Models;
using Digger.Server.Models.Authentification;
using Digger.Server.Models.Project;
using DiStock.DAL;
using DiStock.DAL.Datas.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Digger.Server.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("/api/[controller]")]
    public class UserController : Controller
    {
        readonly UserGateway _userGateway;
        readonly PasswordHasher _passwordHasher;
        readonly GetAccessUser _getAccessUser;

        public UserController(UserGateway userGateway, PasswordHasher passwordHasher, GetAccessUser getAccessUser)
        {
            _userGateway = userGateway;
            _passwordHasher = passwordHasher;
            _getAccessUser = getAccessUser;
        }

        [Authorize(Policy = "admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            IEnumerable<UserData> users = await _userGateway.GetAllUsers();
            if (users == null) return BadRequest("No user exists");

            return Ok(users);
        }
        
        [Authorize]
        [HttpGet("GetUserForInvitByProjectId/{projectId}")]
        public async Task<IActionResult> GetUserForInvitByProjectId(int projectId)
        {
            if (!HttpContext.User.IsInRole("admin"))
            {
                EnumProjectAccessRight accessRight = await _getAccessUser.GetUserAccessRightProject(Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), projectId);
                if (accessRight != EnumProjectAccessRight.Admin) return StatusCode(403, "Access Denied !");
            }

            IEnumerable<UserForInvitData> result = await _userGateway.GetUserForInvitByProjectId(projectId);
            if (result == null) return BadRequest("No user avaibles for invit");

            return Ok(result);
        }

        [Authorize(Policy = "admin")]
        [HttpPut("UpgradeUserInAdmin/{userId}")]
        public async Task<IActionResult> UpgradeUserInAdmin(int userId)
        {
            Result result = await _userGateway.ChangeRank(userId, "admin");
            if (result.ErrorMessage == "User not found") return BadRequest(result.ErrorMessage);

            return Ok("User is now in admin");
        }

        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (!HttpContext.User.IsInRole("admin") && !_getAccessUser.UserCookieIs(HttpContext, Convert.ToString(userId))) return StatusCode(403, "Access Denied !");

            Result result = await _userGateway.DeleteUser(userId);
            if (result.ErrorMessage == "User not found") return BadRequest(result.ErrorMessage);

            // if the user delete his own account
            if (_getAccessUser.UserCookieIs(HttpContext, Convert.ToString(userId))) await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok("User deleted");
        }

        [Authorize]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserViewModel model)
        {
            if (userId == 0) userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!HttpContext.User.IsInRole("admin") && !_getAccessUser.UserCookieIs(HttpContext, Convert.ToString(userId))) return StatusCode(403, "Access Denied !");

            byte[] passwordHash = _passwordHasher.HashPassword(model.Password);
            string role = "user";

            Result result = await _userGateway.UpdateUser(userId, model.Pseudo, passwordHash, role);
            if (result.ErrorMessage == "User id does not exists") return BadRequest(result.ErrorMessage);
            if (result.ErrorMessage == "This pseudo is already used") return BadRequest(result.ErrorMessage);

            return Ok("User updated");
        }
    }
}