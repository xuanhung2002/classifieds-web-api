using Classifieds.Data;
using Classifieds.Services.IServices;

namespace Classifieds_API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, DataContext dataContext, ITokenService tokenService)
        {
            // extract jwt token out of request header
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var accountId = tokenService.ValidateJwtToken(token);
            if (accountId != null)
            {
                // attach account to context on successful jwt validation
                var user = await dataContext.Users.FindAsync(accountId.Value);
                context.Items["User"] = user;
            }

            await _next(context);
        }
    }
}
