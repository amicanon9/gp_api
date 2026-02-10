using Gp_Api.Hubs;
using Gp_Api.IServices;
using Gp_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PMS_api.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskMasterController : ControllerBase
    {
        private readonly ITaskMasterService _service;
        private readonly IFileProcessorService _fileService;
        private readonly IHubContext<ChatHub> _hubContext;

        public TaskMasterController(ITaskMasterService service, IFileProcessorService fileService, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _fileService = fileService;
            _hubContext = hubContext;
        }

        private void SetServiceClaims()
        {
            var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");
            var bookIdClaim = User.Claims.FirstOrDefault(t => t.Type == "book_id");
            var roleIdClaim = User.Claims.FirstOrDefault(t => t.Type == "role_id");

            if (userIdClaim != null) _service.UserId = short.Parse(userIdClaim.Value);
            if (bookIdClaim != null) _service.BookId = short.Parse(bookIdClaim.Value);
            if (roleIdClaim != null) _service.RoleId = int.Parse(roleIdClaim.Value);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                SetServiceClaims();
                return Ok(_service.GetAllData());
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPost]
        public IActionResult Post(TaskMasterView data)
        {
            try
            {
                SetServiceClaims();

                // 接收 Service 回傳的 ID
                int newId = _service.InsertData(data);

                _hubContext.Clients.All.SendAsync("TaskMaster", "新增");

                // 回傳給前端，確保 key 名稱是 id
                return Ok(new { result = "inserted", id = newId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Edit(int id, TaskMasterView data)
        {
            try
            {
                SetServiceClaims();
                var res = _service.EditData(id, data);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("TaskMaster", "更新");
                    return Ok(new { result = "updated" });
                }
                return res == "NotFound" ? NotFound() : BadRequest(new { result = res });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var res = _service.DeleteData(id);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("TaskMaster", "刪除");
                    return Ok(new { result = "deleted" });
                }
                return res == "NotFound" ? NotFound() : BadRequest(new { result = res });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

      
    }
}