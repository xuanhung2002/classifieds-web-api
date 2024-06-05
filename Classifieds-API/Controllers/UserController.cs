using AutoMapper;
using Classifieds.Data.DTOs;
using Classifieds.Data.Enums;
using Classifieds.Services.IServices;
using Classifieds_API.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            //if (id != User.Id && User.Role != Role.Admin)
            //{
            //    return Unauthorized(new { message = "Unauthorize" });
            //}
            var user = await _userService.GetById(id);
            return Ok(user);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            if (User != null)
            {
                var user = _mapper.Map<UserDto>(User);
                return Ok(user);
            }
            else
            {

                return Unauthorized("Invalid token");
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var user = await _userService.GetUsers(s => true);
            return Ok(user);
        }
    }
}
