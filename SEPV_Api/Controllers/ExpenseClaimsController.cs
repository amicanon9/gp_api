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
    public class ExpenseClaimsController : ControllerBase
    {
        private readonly IExpenseClaimsService _service;
        private readonly IHubContext<ChatHub> _hubContext;

        public ExpenseClaimsController(IExpenseClaimsService service, IHubContext<ChatHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");
                var bookIdClaim = User.Claims.FirstOrDefault(t => t.Type == "book_id");
                if (bookIdClaim != null) _service.BookId = Int16.Parse(bookIdClaim.Value);
                if (userIdClaim != null) _service.UserId = Int16.Parse(userIdClaim.Value);

                return Ok(_service.GetAllData());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "讀取資料失敗", error = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");
                var bookIdClaim = User.Claims.FirstOrDefault(t => t.Type == "book_id");
                if (bookIdClaim != null) _service.BookId = Int16.Parse(bookIdClaim.Value);
                if (userIdClaim != null) _service.UserId = Int16.Parse(userIdClaim.Value);

                return Ok(_service.GetDataByUserId(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "讀取資料失敗", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post(ExpenseClaimsView dataList)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(t => t.Type == "user_id");
                var bookIdClaim = User.Claims.FirstOrDefault(t => t.Type == "book_id");

                if (bookIdClaim != null) _service.BookId = Int16.Parse(bookIdClaim.Value);
                if (userIdClaim != null) _service.UserId = Int16.Parse(userIdClaim.Value);

                // 接收新產生的 ID
                int newId = _service.InsertData(dataList);

                _hubContext.Clients.All.SendAsync("ExpenseClaims", "新增");

                // 將 id 回傳，前端才能拿到 res.id
                return Ok(new { result = "inserted", id = newId });
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
                    _hubContext.Clients.All.SendAsync("ExpenseClaims", "刪除");
                    return Ok(new { result = "deleted" });
                }
                return res == "NotFound" ? NotFound(new { result = "找不到資料" }) : BadRequest(new { result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Edit(int id, ExpenseClaimsView data)
        {
            try
            {
                var res = _service.EditData(id, data);
                if (res == "OK")
                {
                    _hubContext.Clients.All.SendAsync("ExpenseClaims", "更新");
                    return Ok(new { result = "updated" });
                }
                return res == "NotFound" ? NotFound(new { result = "找不到資料" }) : BadRequest(new { result = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}