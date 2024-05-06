using Classifieds.Data.Enums;
using Classifieds.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id != User.Id && User.Role != Role.Admin)
            {
                return Unauthorized(new { message = "Unauthorize" });
            }
            var user = await _userService.GetById(id);
            return Ok(user);
        }

        //[HttpGet("username")]
        //public async Task<IActionResult> GetByUsername(string username)
        //{

        //}
    }
}
