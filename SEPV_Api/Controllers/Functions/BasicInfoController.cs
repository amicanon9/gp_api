using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gp_Api.IServices;
using System;
using System.Linq;

namespace Gp_Api.Controllers.Functions
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BasicInfoController : ControllerBase
    {
        private readonly ILoginRolesService _loginRolesService;
        private readonly ILoginMenusService _loginMenusService;
        public BasicInfoController (ILoginRolesService loginRolesService, ILoginMenusService loginMenusService) 
        {
            _loginRolesService = loginRolesService;
            _loginMenusService = loginMenusService;
        }

        // get current user's all roles
        [HttpGet("~/api/getroles")]
        public IActionResult GetUserRoles()
        {
            if (User.HasClaim(a => a.Type == "book_id") && User.HasClaim(a => a.Type == "user_id"))
            {
                _loginRolesService.BookId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "book_id").Value);
                var user_id = Int32.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
                return Ok(_loginRolesService.GetUserRoles(user_id));
            }
            else
            {
                return BadRequest(new { result = "Bad JWT" });
            }
        }

        // get user current role's menus
        [HttpGet("~/api/menus")]
        public IActionResult GetMenusWithRole()
        {
            var user_id = User.Claims.FirstOrDefault(t => t.Type == "user_id").Value;
            var role_id = User.Claims.FirstOrDefault(t => t.Type == "role_id").Value;
            _loginMenusService.BookId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "book_id").Value);
            if (user_id == null) return Unauthorized();
            return Ok(_loginMenusService.GetMenu(Int32.Parse(user_id), Int32.Parse(role_id)));
        }
    }
}
