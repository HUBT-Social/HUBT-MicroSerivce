using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Chat_API.Configurations
{
    public static class JwtConfiguration
    {
        public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSetting? jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSetting>();
            List<string>? hubPaths = configuration.GetSection("SignalRHubs")?.Get<List<string>>() ?? new List<string> { "/chathub" };
            
            if (jwtSettings != null)
            {
                services.ConfigureJwt(jwtSettings, hubPaths);
                return services;
            }
            throw new Exception("Connection fail");
        }
    }
}