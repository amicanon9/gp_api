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
    public class Customer2CheckController : ControllerBase
    {
        private readonly ICustomer2CheckService _service;
        private readonly IHubContext<ChatHub> _hubContext;

        public Customer2CheckController(ICustomer2CheckService service, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_service.GetAllData());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "讀取資料失敗", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post(Customer2CheckView data)
        {
            try
            {
                _service.InsertData(data);
                _hubContext.Clients.All.SendAsync("Customer2Check", "新增");
                return Ok(new { result = "inserted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, Customer2CheckView data)
        {
            try
            {
                var res = _service.EditData(id, data);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("Customer2Check", "編輯");
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
                    _hubContext.Clients.All.SendAsync("Customer2Check", "刪除");
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
