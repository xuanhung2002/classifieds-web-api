using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.PostDTOs;
using Classifieds.Data.Entities;
using Classifieds.Data.Enums;
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

        [HttpPost("paging")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaging([FromForm] PostPagingRequest request)
        {
            var res = await _postService.GetPagingAsync(request);
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
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> Create([FromForm] PostAddDto request)
        {
            await _postService.AddAsync(request);
            return Ok();
        }

        [HttpPut]
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> Update([FromForm]PostUpdateDto dto)
        {
            await _postService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete]
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
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
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> OpenAuction(OpenAuctionDto model)
        {
            await _postService.OpenAuction(model, User.Id);
            return Ok();
        }
        [HttpPut("reopen-auction")]
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> ReOpenAuction(OpenAuctionDto model)
        {
            await _postService.ReOpenAuction(model, User.Id);
            return Ok();
        }

        [HttpPut("close-auction/{id}")]
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> CloseAuction(Guid id)
        {
            await _postService.CloseAuction(id, User.Id);
            return Ok();
        }

        [HttpPut("mark-sold/{id}")]
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> MarkSold(Guid id)
        {
            await _postService.MarkSold(id);
            return Ok();
        }

        [HttpPost("mypost")]
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> GetMyPost([FromForm]PostOfUserRequest request)
        {
            var res = await _postService.GetPostOfCurrentUser(request);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        [Authorize(Role.SuperAdmin, Role.Admin, Role.User)]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            return Ok();
        }
    }
}
