using HUBT_Social_Core.Settings;
using HUBT_Social_Identity_Service.ASP_Extensions;
using HUBT_Social_Identity_Service.Services.IdentityCustomeService;
using Identity_API.Src.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity_API.Configurations;

public static class IdentityConfiguration
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        DatabaseSetting? databaseSetting = configuration.GetSection("AuthService").Get<DatabaseSetting>();

        if (databaseSetting != null)
        {
            services.AddCustomIdentity<AUser, ARole>(databaseSetting).AddTokenManager<AUser,ARole,UserToken>();
            return services;
        }
        Console.WriteLine("Unable to connect to AuthService");
        return services;
    }
}