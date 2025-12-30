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
    public class DropListController : ControllerBase
    {

        // Selectors
        private readonly ICodeLookupService _codeLookupService;
        private readonly ILoginRolesService _loginRolesService;
        private readonly ILoginMenusService _loginMenusService;
        public DropListController(
            ICodeLookupService codeLookupService,
            ILoginRolesService loginRolesService,
            ILoginMenusService loginMenusService)
        {
            _codeLookupService = codeLookupService;
            _loginRolesService = loginRolesService;
            _loginMenusService = loginMenusService;

        }

        [HttpGet("codeLookup/{source}")]
        public IActionResult GetCodeLookup(string source)
        {
            return Ok(_codeLookupService.GetSelector(source));
        }

    

      
        [HttpGet("loginRoles")]
        public IActionResult GetLoginRoles()
        {
            _loginRolesService.BookId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "book_id").Value);
            return Ok(_loginRolesService.GetSelector());
        }

   

        [HttpGet("loginMenus")]
        public IActionResult GetLoginMenus()
        {
            _loginMenusService.UserId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
            _loginMenusService.RoleId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "role_id").Value);
            return Ok(_loginMenusService.GetLoginMenus());
        }

    
    }
}
