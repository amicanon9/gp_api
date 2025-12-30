using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SEPV_Api.Models.GreenPower;
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
    // [Authorize]
    public class BankInfoController : ControllerBase
    {
        
        private readonly IBankInfoService _service;
        private readonly IHubContext<ChatHub> _hubContext;
        public BankInfoController(IBankInfoService service, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            _service.RoleId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "role_id").Value);
            _service.UserId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
            return Ok(_service.GetAllData());
        }
     

        // POST /api/BankInfo/2
        [HttpPost]
        public IActionResult Post(BankInfoView data)
        {
            try
            {
                _service.InsertData(data);
                _hubContext.Clients.All.SendAsync("BankInfo", "新增");
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
                    _hubContext.Clients.All.SendAsync("BankInfo", "刪除");
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
        public IActionResult Edit(int id, BankInfoView data)
        {
            try
            {
                var res = _service.EditData(id, data);
                if (res=="OK")
                {
                    _hubContext.Clients.All.SendAsync("BankInfo", "更新");
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
