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
    public class PpBasicInfoController : ControllerBase
    {
        
        private readonly IPpbasicInfoService _service;
        private readonly IHubContext<ChatHub> _hubContext;
        public PpBasicInfoController(IPpbasicInfoService service, IHubContext<ChatHub> hubContext)
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
     

        // POST /api/PpbasicInfo/2
        [HttpPost]
        public IActionResult Post(PpbasicInfoView data)
        {
            try
            {
                _service.InsertData(data);
                _hubContext.Clients.All.SendAsync("PpBasicInfo", "新增");
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
                    _hubContext.Clients.All.SendAsync("PpBasicInfo", "刪除");
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
        public IActionResult Edit(int id, PpbasicInfoView data)
        {
            try
            {
                var res = _service.EditData(id, data);
                if (res=="OK")
                {
                    _hubContext.Clients.All.SendAsync("PpBasicInfo", "更新");
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
