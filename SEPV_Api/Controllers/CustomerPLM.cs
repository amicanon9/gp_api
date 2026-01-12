using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SEPV_Api.Models.GreenPower;
using Gp_Api.Hubs;
using Gp_Api.IServices;
using Gp_Api.Services; // 確保引用了包含 CustomerPlmView 的命名空間
using System;
using System.Linq;

namespace Gp_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerPlmController : ControllerBase
    {
        private readonly ICustomerPlmService _service;
        private readonly IHubContext<ChatHub> _hubContext;

        public CustomerPlmController(ICustomerPlmService service, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // 從 Token 取得 User 資訊並賦值給 Service
                var roleIdClaim = User.Claims.FirstOrDefault(t => t.Type == "role_id");
                var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");

                if (roleIdClaim != null) _service.RoleId = Int16.Parse(roleIdClaim.Value);
                if (userIdClaim != null) _service.UserId = Int16.Parse(userIdClaim.Value);

                return Ok(_service.GetAllData());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "讀取資料失敗", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post(CustomerPlmView data)
        {
            try
            {
                _service.InsertData(data);
                // 透過 SignalR 通知前端更新
                _hubContext.Clients.All.SendAsync("CustomerPlm", "新增");
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
                var res = _service.DeleteData(id);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("CustomerPlm", "刪除");
                    return Ok(new { result = "deleted" });
                }
                else if (res == "NotFound")
                {
                    return NotFound(new { result = "找不到該筆資料" });
                }
                return BadRequest(new { result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Edit(int id, CustomerPlmView data)
        {
            try
            {
                var res = _service.EditData(id, data);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("CustomerPlm", "更新");
                    return Ok(new { result = "updated" });
                }
                else if (res == "NotFound")
                {
                    return NotFound(new { result = "找不到該筆資料" });
                }
                return BadRequest(new { result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}