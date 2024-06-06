using Classifieds.Data.DTOs.ChatDtos;
using Classifieds.Services.IServices;
using Classifieds_API.Authorization;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : BaseController
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _chatService.GetChatByUserId(User.Id);
            return Ok(res);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateChatRequest model)
        {
            var res = await _chatService.Create(model.PartnerId, User.Id);
            return Ok(res.CreateChatResponseModels);
        }

        [HttpGet("messages/{id:Guid}/{page:Guid}")]
        public async Task<IActionResult> GetMessages(Guid id, Guid page)
        {
            var res = await _chatService.Messages(id, page);
            return Ok(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _chatService.Delete(id);
            return Ok(res);
        }

        //// Optionally include if image uploading is needed
        //[HttpPost("upload-image")]
        //public IActionResult UpLoadImage([FromForm] UploadImageRequest model)
        //{
        //    using var stream = model.image.OpenReadStream();
        //    var res = _chatService.UpLoadImage(stream);
        //    return Ok(res);
        //}
    }
