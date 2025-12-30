using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gp_Api.Helpers;
using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;


namespace Gp_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly JwtHelpers _jwtHelpers;
        private readonly ITokenService _tokenService;

        public TokenController(JwtHelpers jwtHelpers, ITokenService tokenService)
        {
            _jwtHelpers = jwtHelpers;
            _tokenService = tokenService;
        }


        [HttpPost("~/api/login")]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            if (_tokenService.ValidateUser(loginViewModel))
            {
                var data = _tokenService.GetData(loginViewModel.Username);
                var role_id = _tokenService.GetRoleId((int)data.Id);
                var role = _tokenService.GetLoginRoles(role_id);
                return Ok(new { token = _jwtHelpers.GenerateToken(role_id, (int)data.Id,data.Username,
                    data.Book_id,role.PermissionLevel, data.Company_name) });
            }
            else
            {
                return Forbid();
            }
        }
        [HttpPost("~/api/refreshToken")]
        public IActionResult RefreshToken(RefreshTokenViewModel viewModel)
        {
            return Ok(new { token = _jwtHelpers.RefreshToken(viewModel.username, viewModel.token) });
        }
        [Authorize]
        [HttpPost("~/api/switchRole")]
        public IActionResult SwitchRole(SwitchRoleViewModel viewModel)
        {
            var data = _tokenService.GetData(User.Identity.Name);
            var role_id = _tokenService.SwitchRoleId((int)data.Id, viewModel.role_id);
            if (role_id == -1) return Unauthorized();
            var role = _tokenService.GetLoginRoles(role_id);
            return Ok(new { token = _jwtHelpers.GenerateToken( role_id,
                (int)data.Id, data.Username, data.Book_id, role.PermissionLevel, data.Company_name) });
        }
    }

    public class RefreshTokenViewModel
    {
        public string username { get; set; }
        public string token { get; set; }
    }
}
