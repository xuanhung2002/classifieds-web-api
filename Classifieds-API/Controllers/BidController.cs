using Classifieds.Data.DTOs.BidDtos;
using Classifieds.Data.Enums;
using Classifieds.Services.IServices;
using Classifieds.Services.Services;
using Classifieds_API.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidController : BaseController
    {
        private readonly IBidService _bidService;

        public BidController(IBidService bidService)
        {
            _bidService = bidService;
        }

        [HttpPost]
        [Authorize(Role.User, Role.Admin)]
        public async Task<IActionResult> CreateBid(CreateBidRequest request)
        {
            var res = await _bidService.CreateBidAsync(request, User.Id);
            return Ok(res);
        }
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetBidsOfPost(Guid postId)
        {
            var res = await _bidService.GetBidsOfPost(postId);
            return Ok(res);
        }

    }
}
