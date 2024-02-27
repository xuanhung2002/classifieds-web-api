using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Classifieds_API.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters()
                  {
                      ValidateIssuer = false,
                      ValidateAudience = false,
                      ValidateLifetime = false,
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes(configuration["JwtSecretKey"]!))
                  };
              });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("RequireAdminRole", policy =>
                //    policy.RequireRole(Roles.Admin.ToString()));               
            });
        }

        public static void AddServices(this IServiceCollection services)
        {
        }
    }
}
