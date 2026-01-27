using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Gp_Api.Hubs;
using Gp_Api.IServices;
using Gp_Api.Services;
using System;
using System.Linq;

namespace Gp_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentsService _service;
        private readonly IHubContext<ChatHub> _hubContext;

        public DepartmentsController(IDepartmentsService service, IHubContext<ChatHub> hubContext)
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
        [HttpGet("{id}")]
        public IActionResult GetByProject(int id)
        {
            try
            {
                var roleIdClaim = User.Claims.FirstOrDefault(t => t.Type == "role_id");
                if (roleIdClaim != null) _service.RoleId = Int16.Parse(roleIdClaim.Value);

                // 取得特定專案的週報
                return Ok(_service.GetDataById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "讀取週報失敗", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post(DepartmentsView data)
        {
            try
            {
                var roleIdClaim = User.Claims.FirstOrDefault(t => t.Type == "role_id");
                if (roleIdClaim != null) _service.RoleId = Int16.Parse(roleIdClaim.Value);

                _service.InsertData(data);
                _hubContext.Clients.All.SendAsync("Departments", "新增");
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
                    _hubContext.Clients.All.SendAsync("Departments", "刪除");
                    return Ok(new { result = "deleted" });
                }
                return res == "NotFound" ? NotFound(new { result = "找不到該週報" }) : BadRequest(new { result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Edit(int id, DepartmentsView data)
        {
            try
            {
                var res = _service.EditData(id, data);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("Departments", "更新");
                    return Ok(new { result = "updated" });
                }
                return res == "NotFound" ? NotFound(new { result = "找不到該週報" }) : BadRequest(new { result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}