using Classifieds.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Classifieds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly INotificationSerivce _notificationSerivce;
        public NotificationController(INotificationSerivce notificationSerivce)
        {
            _notificationSerivce = notificationSerivce;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var res = await _notificationSerivce.GetByUserIdAsync(userId);
            if (res.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status204NoContent, "No notification");
            }
            return Ok(res);
        }
    }
}
