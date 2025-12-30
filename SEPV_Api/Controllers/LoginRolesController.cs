using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Gp_Api.Hubs;
using Gp_Api.IServices;
using Gp_Api.Middlewares;
using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Controllers
{
    [Authorize]
   
    [Route("api/[controller]")]
    [ApiController]
    public class LoginRolesController : ControllerBase
    {
        private readonly ILoginRolesService _service;
        private readonly IHubContext<ChatHub> _hubContext;
        public LoginRolesController(ILoginRolesService service, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (User.HasClaim(a => a.Type == "book_id")) {
                _service.UserId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
                _service.RoleId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "role_id").Value);
                return Ok(_service.GetAllData());
            }
            else
            {
                return BadRequest(new { result = "Bad JWT" });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (User.HasClaim(a => a.Type == "book_id"))
            {
                _service.UserId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
                _service.RoleId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "role_id").Value);
                return Ok(_service.GetData(id));
            }
            else
            {
                return BadRequest(new { result = "Bad JWT" });
            }
        }

        [HttpPost]
        public IActionResult Post(LoginRolesViewModel viewModel)
        {
         
            try
            {
                _service.UserId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
                _service.RoleId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "role_id").Value);
                _service.InsertData(viewModel);
                _hubContext.Clients.All.SendAsync("login_role", "新增");
                return Ok(new { result = "inserted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var res =_service.DeleteData(id);
                if(res=="OK")
                {
                 
                    _hubContext.Clients.All.SendAsync("login_role", "刪除");
                    return Ok(new { result = "deleted" });
                }
                else return BadRequest(new { result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Edit(int id, LoginRolesViewModel viewModel)
        {
           
            try
            {
                _service.UserId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
                _service.RoleId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "role_id").Value);
                var res = _service.EditData(id, viewModel);
                if (res == "OK")
                {
                
                    _hubContext.Clients.All.SendAsync("login_role", "更新");
                    return Ok(new { result = "updated" });
                }
                else return BadRequest(new { result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
