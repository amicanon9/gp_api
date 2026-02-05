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
                _service.InsertData(data);
                _hubContext.Clients.All.SendAsync("TaskMaster", "新增");
                return Ok(new { result = "inserted", id = data.id });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
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

        #region 檔案處理動態端點 (配合 FileUploadComponent)

        // 取得檔案清單 (組件重新整理用)
        [HttpGet("{id}/images/{category}")]
        public IActionResult GetFileList(int id, string category)
        {
            try
            {
                var files = _fileService.GetDataNameList(id, category);
                return Ok(files);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // 上傳檔案
        [HttpPost("{id}/images/{category}")]
        public IActionResult UploadImages(int id, string category, List<IFormFile> files)
        {
            var result = _fileService.UploadData(files, id, category);
            if (result.Status == "Ok")
            {
                _hubContext.Clients.All.SendAsync("TaskMaster", "圖片更新");
                return Ok(result);
            }
            return BadRequest(result);
        }

        // 下載檔案
        [HttpGet("{id}/images/{category}/{fileName}")]
        public IActionResult DownloadImage(int id, string category, string fileName)
        {
            var result = _fileService.DownloadData(fileName, id, category);
            if (result == null) return NotFound();
            return File(result.File_byte, result.Option, result.File_name);
        }

        // 刪除檔案
        [HttpDelete("{id}/images/{category}/{fileName}")]
        public IActionResult DeleteImage(int id, string category, string fileName)
        {
            var result = _fileService.DeleteData(fileName, id, category);
            if (result.Status == "Ok")
            {
                _hubContext.Clients.All.SendAsync("TaskMaster", "圖片刪除");
                return Ok(result);
            }
            return BadRequest(result);
        }

        #endregion
    }
}