using Classifieds.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.Services
{
    public interface ICurrentUserService
    {
        public User? User { get; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public User? User
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var user = httpContext.Items["User"] as User;
                return user;
            }
        }
    }
}
