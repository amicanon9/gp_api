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
    public class CheckinLogsController : ControllerBase
    {
        private readonly ICheckinLogsService _service;
        private readonly IHubContext<ChatHub> _hubContext;

        public CheckinLogsController(ICheckinLogsService service, IHubContext<ChatHub> hubContext)
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
                var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");

                if (roleIdClaim != null) _service.RoleId = Int16.Parse(roleIdClaim.Value);
                if (userIdClaim != null) _service.UserId = Int16.Parse(userIdClaim.Value);

                return Ok(_service.GetDataById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "讀取週報失敗", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post(CheckinLogsView data)
        {
            try
            {
                var roleIdClaim = User.Claims.FirstOrDefault(t => t.Type == "role_id");
                var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");

                if (roleIdClaim != null) _service.RoleId = Int16.Parse(roleIdClaim.Value);
                if (userIdClaim != null) _service.UserId = Int16.Parse(userIdClaim.Value);

                _service.InsertData(data);
                _hubContext.Clients.All.SendAsync("CheckinLogs", "新增");
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
                    _hubContext.Clients.All.SendAsync("CheckinLogs", "刪除");
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
        public IActionResult Edit(int id, CheckinLogsView data)
        {
            try
            {
                var res = _service.EditData(id, data);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("CheckinLogs", "更新");
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