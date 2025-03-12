using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Notation_API.Configurations
{
    public static class JwtConfiguration
    {
        public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSetting? jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSetting>();

            if (jwtSettings != null)
            {
                services.ConfigureJwt(jwtSettings);
                return services;
            }
            throw new Exception("Connection fail");
        }
    }
}
