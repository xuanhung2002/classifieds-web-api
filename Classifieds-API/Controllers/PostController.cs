using Classifieds.Data.DTOs;
using Classifieds.Data.Models;
using Classifieds.Repository;
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
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var res = await _postService.GetAllAsync();
            return Ok(res);
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var res = await _postService.GetByIdAsync(id);
            return Ok(res);
        }

        [HttpPost]
        [Authorize(Role.User, Role.Admin)]
        public async Task<IActionResult> Create(PostAddDto dto)
        {
            dto.UserId = User.Id;
            await _postService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        [Authorize(Role.User, Role.Admin)]
        public async Task<IActionResult> Update(PostUpdateDto dto)
        {
            await _postService.UpdateAsync(dto, User.Id);
            return Ok();
        }

        [HttpDelete]
        [Authorize(Role.User, Role.Admin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post != null)
            {
                if(post.UserId != User.Id && User.Role != Role.Admin)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "Forbidden");
                }
                await _postService.DeleteAsync(id);
            }
            return BadRequest(new {message = "Post is not existed"});
        }

        [HttpPut("open-auction")]
        [Authorize(Role.User, Role.Admin)]
        public async Task<IActionResult> OpenAuction(OpenAuctionDto model)
        {
            await _postService.OpenAuction(model, User.Id);
            return Ok();
        }

        [HttpPut("close-auction")]
        [Authorize(Role.User, Role.Admin)]
        public async Task<IActionResult> CloseAuction(Guid id)
        {
            await _postService.CloseAuction(id, User.Id);
            return Ok();
        }
    }
}
