using Classifieds.Data.DTOs.UserDtos;
using Classifieds.Data.Enums;
using Classifieds.Services.IServices;
using Classifieds_API.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperAdminController : BaseController
    {
        private readonly IUserService _userService;
        public SuperAdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("admin")]
        [Authorize(Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> GetAllAdmin()
        {
            var admins = await _userService.GetUsers(s => s.Role == Role.Admin);
            return Ok(admins);
        }
        [HttpPut("grant-permission")]
        [Authorize(Role.SuperAdmin)]
        public async Task<IActionResult> GrantPermisson(Guid userId, Role role)
        {
            await _userService.ChangePermission(userId, role);
            return Ok();
        }

        [HttpPost("add-admin")]
        public async Task<IActionResult> AddAdminAccount(RegisterDto dto)
        {
            await _userService.AddAdmin(dto);
            return Ok();
        }


    }
}
