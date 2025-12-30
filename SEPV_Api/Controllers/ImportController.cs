using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SEPV_Api.Models.GreenPower;
using Gp_Api.Hubs;
using Gp_Api.IServices;
using Gp_Api.Services;
using System;
using System.Linq;
using static Gp_Api.Services.res;
using System.Collections.Generic;


namespace Gp_Api.Controllers
{
    [Authorize]
   
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class ImportController : ControllerBase
    {
        
        private readonly IImportService _service;
        private readonly IHubContext<ChatHub> _hubContext;
        public ImportController(IImportService service, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }


        // POST /api/Import/2
        [HttpPost("service")]
        public IActionResult service(List<Service> data)
        {
            try
            {
                _service.BookId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "book_id").Value);
                _service.RoleId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "role_id").Value);
                _service.UserId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
                var res = _service.ImportServiceData(data);
                _hubContext.Clients.All.SendAsync("Import", "新增");
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/Import/2
        [HttpPost("ps")]
        public IActionResult ps(ImportObj data)
        {
            try
            {
                _service.BookId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "book_id").Value);
                _service.RoleId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "role_id").Value);
                _service.UserId = Int16.Parse(User.Claims.FirstOrDefault(t => t.Type == "user_id").Value);
                var res= _service.ImportData(data);
                _hubContext.Clients.All.SendAsync("Import", "新增");
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

      
    }
}
