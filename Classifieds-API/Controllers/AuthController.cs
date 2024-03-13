using Classifieds.Data.DTOs;
using Classifieds.Services.IServices;
using Classifieds.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        { 
            var token = await _authService.Login(loginDto);
            if(token == null)
            {
                return Unauthorized("Login failed");
            }

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
           var res = await _authService.Register(registerDto);
            return res ? Ok("Register successfully") : BadRequest("Register fail");
        }
    }
}
