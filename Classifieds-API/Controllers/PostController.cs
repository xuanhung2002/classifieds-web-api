using Classifieds.Data.DTOs;
using Classifieds.Services.IServices;
using Classifieds_API.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : BaseController
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(User.AccountName);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(AddPostDto dto)
        {
            dto.UserId = User.Id;
            await _postService.AddAsync(dto);
            return Ok();
        }
    }
}
