using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Gp_Api.Hubs;
using Gp_Api.Services;
using System;
using System.Linq;

namespace Gp_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class Project2CheckController : ControllerBase
    {
        private readonly IProject2CheckService _service;
        private readonly IHubContext<ChatHub> _hubContext;

        public Project2CheckController(IProject2CheckService service, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var roleIdClaim = User.Claims.FirstOrDefault(t => t.Type == "role_id");
                var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");
                var bookIdClaim = User.Claims.FirstOrDefault(t => t.Type == "book_id");
                if (roleIdClaim != null) _service.RoleId = Int16.Parse(roleIdClaim.Value);
                if (userIdClaim != null) _service.UserId = Int16.Parse(userIdClaim.Value);
                if (bookIdClaim != null) _service.BookId = Int16.Parse(bookIdClaim.Value);
                return Ok(_service.GetAllData());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "讀取資料失敗", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post(Project2CheckView data)
        {
            try
            {
                var roleIdClaim = User.Claims.FirstOrDefault(t => t.Type == "role_id");
                if (roleIdClaim != null) _service.RoleId = Int16.Parse(roleIdClaim.Value);
                var bookIdClaim = User.Claims.FirstOrDefault(t => t.Type == "book_id");
                if (bookIdClaim != null) _service.BookId = Int16.Parse(bookIdClaim.Value);
                _service.InsertData(data);
                _hubContext.Clients.All.SendAsync("Project2Check", "新增");
                return Ok(new { result = "inserted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, Project2CheckView data)
        {
            try
            {
                var res = _service.EditData(id, data);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("Project2Check", "編輯");
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

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var res = _service.DeleteData(id);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("Project2Check", "刪除");
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
    }
}
