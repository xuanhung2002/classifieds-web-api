using Classifieds.Data.DTOs.WatchListDtos;
using Classifieds.Data.Enums;
using Classifieds.Services.IServices;
using Classifieds_API.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchListController : BaseController
    {

        private readonly IWatchListService _watchListService;
        public WatchListController(IWatchListService watchListService)
        {
            _watchListService = watchListService;
        }

        [HttpPost]
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> AddPostToWatchList(AddWatchPostDto dto)
        {
            await _watchListService.AddWatchPost(dto);
            return Ok();
        }

        [HttpGet]
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> GetWatchList()
        {
            
            var res = await _watchListService.GetWatchListOfCurrentUser();
            return Ok(res);
        }

    }
}
