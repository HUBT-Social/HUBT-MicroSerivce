using HUBT_Social_Base.ASP_Extentions;
using User_API.Src.Service;

namespace User_API.Configurations
{
    public static class HttpClientRegister
    {
        public static IServiceCollection HttpClientRegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClientService();
            string? identityPath = configuration.GetSection("IdentityApi").Get<string>();
            if (identityPath != null)
            {
                services.AddRegisterClientService<IUserService, UserService>(identityPath);
                return services;
            }
            throw new Exception("Unfound Section");
        }
    }
}
