using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SEPV_Api.Models.PMS;
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
    public class ProjectFirmController : ControllerBase
    {
        private readonly IProjectFirmService _service;
        private readonly IHubContext<ChatHub> _hubContext;

        public ProjectFirmController(IProjectFirmService service, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // 從 Token 取得資訊並賦值給 Service
                var roleIdClaim = User.Claims.FirstOrDefault(t => t.Type == "role_id");
                var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");
                var bookIdClaim = User.Claims.FirstOrDefault(t => t.Type == "book_id");

                if (roleIdClaim != null) _service.RoleId = short.Parse(roleIdClaim.Value);
                if (userIdClaim != null) _service.UserId = short.Parse(userIdClaim.Value);
                if (bookIdClaim != null) _service.BookId = short.Parse(bookIdClaim.Value);

                return Ok(_service.GetAllData());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "讀取資料失敗", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post(ProjectFirmView data)
        {
            try
            {
                var roleIdClaim = User.Claims.FirstOrDefault(t => t.Type == "role_id");
                var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");
                var bookIdClaim = User.Claims.FirstOrDefault(t => t.Type == "book_id");

                if (roleIdClaim != null) _service.RoleId = short.Parse(roleIdClaim.Value);
                if (userIdClaim != null) _service.UserId = short.Parse(userIdClaim.Value);
                if (bookIdClaim != null) _service.BookId = short.Parse(bookIdClaim.Value);

                _service.InsertData(data);

                // 透過 SignalR 通知前端更新 (頻道名稱改為 ProjectFirm)
                _hubContext.Clients.All.SendAsync("ProjectFirm", "新增");

                return Ok(new { result = "inserted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Edit(int id, ProjectFirmView data)
        {
            try
            {
                var roleIdClaim = User.Claims.FirstOrDefault(t => t.Type == "role_id");
                var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");
                var bookIdClaim = User.Claims.FirstOrDefault(t => t.Type == "book_id");

                if (roleIdClaim != null) _service.RoleId = short.Parse(roleIdClaim.Value);
                if (userIdClaim != null) _service.UserId = short.Parse(userIdClaim.Value);
                if (bookIdClaim != null) _service.BookId = short.Parse(bookIdClaim.Value);
                var res = _service.EditData(id, data);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("ProjectFirm", "更新");
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
                    _hubContext.Clients.All.SendAsync("ProjectFirm", "刪除");
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