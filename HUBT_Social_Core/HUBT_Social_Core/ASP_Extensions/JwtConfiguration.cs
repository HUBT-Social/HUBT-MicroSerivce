using System.Text;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HUBT_Social_Core.ASP_Extensions;

public static class JwtConfiguration
{
    public static IServiceCollection ConfigureJwt(this IServiceCollection services, JwtSetting jwtSettings)

    {
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
}