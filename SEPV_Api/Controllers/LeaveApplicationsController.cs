using Gp_Api.Hubs;
using Gp_Api.IServices;
using Gp_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveApplicationsController : ControllerBase
    {
        private readonly ILeaveApplicationsService _service;
        private readonly IHubContext<ChatHub> _hubContext;

        public LeaveApplicationsController(ILeaveApplicationsService service, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        // 注入 User Claims 資訊到 Service
        private void SetServiceUserContext()
        {
            _service.UserId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
            _service.RoleId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "role_id").Value);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                SetServiceUserContext();
                return Ok(_service.GetAllData());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "讀取請假紀錄失敗", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                SetServiceUserContext();
                return Ok(_service.GetDataById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "讀取個人請假資料失敗", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post(LeaveApplicationsView data)
        {
            try
            {
                if (data == null) return BadRequest("無資料");

                SetServiceUserContext();

                var res = _service.InsertData(data);
                if (res == "OK")
                {
                    // 通知前端（例如主管端的通知小鈴鐺）
                    _hubContext.Clients.All.SendAsync("LeaveApplications", "新申請");
                    return Ok(new { result = "inserted" });
                }

                return BadRequest(new { result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateStatus(int id, [FromBody] dynamic updateData)
        {
            try
            {
                SetServiceUserContext();
                string status = updateData.status;
                string remark = updateData.manager_remark;

                var res = _service.UpdateStatus(id, status, remark);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("LeaveApplications", "狀態更新");
                    return Ok(new { result = "updated" });
                }

                return res == "NotFound" ? NotFound(new { result = "找不到該申請紀錄" }) : BadRequest(new { result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}