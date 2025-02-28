using System.Text;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HUBT_Social_Core.ASP_Extensions;
{
    public static class JwtConfiguration
    {
        // Phiên bản 1: Chỉ nhận configuration
        public static IServiceCollection ConfigureJwt(this IServiceCollection services,JwtSetting jwtSetting)
        {
            return ConfigureJwt(services, jwtSetting, null); 
        }

        // Phiên bản 2: Nhận configuration và hubPaths
        public static IServiceCollection ConfigureJwt(this IServiceCollection services,JwtSetting jwtSetting,List<string>? hubPaths = null)
        {  
            hubPaths ??= new List<string>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };

                // Cấu hình cho SignalR nếu có hubPaths
                if (hubPaths.Any())
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) && IsSignalRHubPath(path, hubPaths))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                }
            })
            .AddCookie(IdentityConstants.ApplicationScheme);

            
            services.Configure<JwtSetting>(options =>
            {
                options.Issuer = jwtSettings.Issuer;
                options.Audience = jwtSettings.Audience;
                options.SecretKey = jwtSettings.SecretKey;
                options.RefreshSecretKey = jwtSettings.RefreshSecretKey;
                options.TokenExpirationInMinutes = jwtSettings.TokenExpirationInMinutes;
                options.RefreshTokenExpirationInDays = jwtSettings.RefreshTokenExpirationInDays;
            });

            return services;
        }

        private static bool IsSignalRHubPath(PathString path, List<string> hubPaths)
        {
            return hubPaths.Any(hubPath => path.StartsWithSegments(hubPath));
        }
    }
}