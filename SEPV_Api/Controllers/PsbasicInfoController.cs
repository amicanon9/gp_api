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
    // [Authorize]
    public class PsBasicInfoController : ControllerBase
    {
        
        private readonly IPsbasicInfoService _service;
        private readonly IHubContext<ChatHub> _hubContext;
        public PsBasicInfoController(IPsbasicInfoService service, IHubContext<ChatHub> hubContext)
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
     

        // POST /api/PSBasicInfo/2
        [HttpPost]
        public IActionResult Post(PsbasicInfoView data)
        {
            try
            {
                _service.InsertData(data);
                _hubContext.Clients.All.SendAsync("PsbasicInfo", "新增");
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
                    _hubContext.Clients.All.SendAsync("PsbasicInfo", "刪除");
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
        public IActionResult Edit(int id, PsbasicInfoView data)
        {
            try
            {
                var res = _service.EditData(id, data);
                if (res=="OK")
                {
                    _hubContext.Clients.All.SendAsync("PsbasicInfo", "更新");
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
