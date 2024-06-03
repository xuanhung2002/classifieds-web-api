using Classifieds.Data.Enums;
using Classifieds.Services.IServices;
using Classifieds_API.Authorization;
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

        [HttpGet]
        [Authorize(Role.User, Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> GetByUserId()
        {
            var res = await _notificationSerivce.GetByUserIdAsync(User.Id);
            if (res.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status204NoContent, "No notification");
            }
            return Ok(res);
        }
    }
}
