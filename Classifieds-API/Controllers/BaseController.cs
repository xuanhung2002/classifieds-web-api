using Classifieds.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        public User User => (User)HttpContext.Items["User"];
    }
}
