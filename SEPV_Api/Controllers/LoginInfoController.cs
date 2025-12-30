using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class LoginInfoController : ControllerBase
    {
        private readonly ILoginInfoService _service;
        private readonly IHubContext<ChatHub> _hubContext;
        public LoginInfoController(ILoginInfoService service, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        
        [HttpGet]
        public IActionResult GetAll()
        {
            if (User.HasClaim(a => a.Type == "book_id"))
            {
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
        public IActionResult Post(LoginInfoViewModel viewModel)
        {
            var bookId = User.Claims.FirstOrDefault(t => t.Type == "book_id").Value;
            try
            {
                _service.InsertData(viewModel);
                _hubContext.Clients.All.SendAsync("login_info", "新增");
                return Ok(new { result = "inserted" });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var res = _service.DeleteData(id);
                if (res=="OK") {

                  
                    _hubContext.Clients.All.SendAsync("login_info", "刪除");
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
        public IActionResult Edit(int id, LoginInfoViewModel viewModel)
        {
          
            try
            {
                var res = _service.EditData(id, viewModel);
                if (res == "OK")
                {

                    _hubContext.Clients.All.SendAsync("login_info", "更新");
                    return Ok(new { result = "updated" });
                }
                else return BadRequest(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("changePassword")]
        public IActionResult ChangePassword (ChangePasswordViewModel viewModel)
        {
            try
            {
                var id = Int32.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
                // TODO: 透過api取得admin權限的所有角色
                if (_service.ChangePassword(id, viewModel, User.IsInRole("admin")))
                {
                    return Ok(new { result = "Password Changed" });
                }
                else
                {
                    return BadRequest(new { result = "Password Change faild" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
