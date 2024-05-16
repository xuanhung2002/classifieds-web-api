using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.GoogleAuthDtos;
using Classifieds.Data.DTOs.UserDtos;
using Classifieds.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        { 
            var token = await _authService.Login(loginDto);
            if(token == null)
            {
                return Unauthorized("Login failed");
            }
            _logger.LogInformation($"{loginDto.AccountName} logged in");
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
           var res = await _authService.Register(registerDto);
            return res ? Ok("Register successfully") : BadRequest("Register fail");
        }

        [AllowAnonymous]
        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle(GoogleLoginRequest request)
        {
            var token = await _authService.LoginWithGoogle(request);
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            await _authService.ForgotPassword(request);
            return Ok( new {message = "Please check your email for password reset instructions"});
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            await _authService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, you can now login" });
        }
    }

}
