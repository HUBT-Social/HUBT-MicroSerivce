using Auth_API.Src.Services.Identity;
using Auth_API.Src.Services.Postcode;
using Auth_API.Src.Services.TempUser;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core.Settings;

namespace Auth_API.Configuration
{
    public static class HttpClientRegister
    {
        public static IServiceCollection HttpClientRegisterConfiguration(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddHttpClientService();
            string? identityPath = configuration.GetSection("IdentityApi").Get<string>();
            string? tempUserPath = configuration.GetSection("TempUserApi").Get<string>();
            string? postcodePath = configuration.GetSection("PostcodeApi").Get<string>();
            if (identityPath != null)
                services.AddRegisterClientService<IAuthService, AuthService>(identityPath);
            if (tempUserPath != null)
                services.AddRegisterClientService<ITempUserRegister, TempUserRegister>(tempUserPath);
            if (postcodePath != null)
                services.AddRegisterClientService<IPostcodeService, PostcodeService>(postcodePath);

            return services;
        }
    }
}
