using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class CodeLookupSourceController : ControllerBase
    {
        private readonly ICodeLookupSourceService _service;

        public CodeLookupSourceController(ICodeLookupSourceService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAllData());
        }

        [HttpGet("{source}")]
        public IActionResult GetById(string source)
        {
            return Ok(_service.GetData(source));
        }

        [HttpPost]
        public IActionResult Post(CodeLookupSourceViewModel codeLookupViewModel)
        {
            try
            {
                _service.InsertData(codeLookupViewModel);
                return Ok(new { result = "inserted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

      
    }
}
