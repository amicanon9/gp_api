using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Gp_Api.Hubs;
using Gp_Api.IServices;
using Gp_Api.Middlewares;
using Gp_Api.Models.ViewModels;
using System;
using System.Linq;


namespace Gp_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class SetOfBooksController : ControllerBase
    {
        private readonly ISetOfBooksService _service;
        private readonly IHubContext<ChatHub> _hubContext;
        public SetOfBooksController(ISetOfBooksService service, IHubContext<ChatHub> hubContext)
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
        // /api/setofbooks/10 
        [HttpGet("{id}")]
        //[ActionName("GetById")]
        public IActionResult GetById(short id)
        {
            return Ok(_service.GetData(id));
        }

        
        [HttpPost]
        public IActionResult Post(SetOfBooksViewModel setOfBooksViewModel)
        {
            try
            {
                _service.InsertData(setOfBooksViewModel);
                _hubContext.Clients.All.SendAsync("books", "新增");
                return Ok(new { result = "inserted"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpDelete("{id}")]
        //[Route("api/[controller]/{id}")]
        public IActionResult Delete(short id)
        {
            try
            {
                var res =_service.DeleteData(id);
                if(res=="OK")
                {
                    _hubContext.Clients.All.SendAsync("books", "刪除");
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
        public IActionResult Edit(short id, SetOfBooksViewModel setOfBooksViewModel)
        {
            try
            {
                var res = _service.EditData(id, setOfBooksViewModel);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("books", "更新");
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