using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gp_Api.IServices;
using Gp_Api.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Controllers
{
    [Authorize]
   
    [Route("api/[controller]")]
    [ApiController]
    public class LoginMenusController : ControllerBase
    {
        private readonly ILoginMenusService _service;
        public LoginMenusController(ILoginMenusService service)
        {
            _service = service;
        }
    }
}
