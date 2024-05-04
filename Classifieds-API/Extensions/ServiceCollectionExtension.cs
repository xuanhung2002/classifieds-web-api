using Classifieds.Repository;
using Classifieds.Services.IServices;
using Classifieds.Services.Services;
using CloudinaryDotNet;
using Common;
using Common.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Classifieds_API.Extensions
{
    public static class ServiceCollectionExtension
    {

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IDBRepository, DBRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IEmailService, EmailService>();
        }
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
                          Encoding.UTF8.GetBytes(AppSettings.JwtSecretKey!))
                  };
              });

            services.AddAuthorization();
        }


        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API",
                    Version = "v1"
                });
                // To Enable authorization using Swagger (JWT)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ8\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public static void AddCloudinary(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new Cloudinary(new Account(
                AppSettings.CloudName,
                AppSettings.ApiKey,
                AppSettings.ApiSecret)));
        }

        public static void AddAutoMapper(this IServiceCollection service)
        {
            var mapper = MappingConfig.RegisterMap().CreateMapper();
            service.AddSingleton(mapper);
        }

        public static void AddCORS(this IServiceCollection service)
        {
            service.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .WithOrigins(AppSettings.CORS)
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .AllowAnyHeader();
                    });
            });

        }
    }
}
