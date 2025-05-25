using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using Notation_API.Src.Services;

namespace Notation_API.Configurations
{
    public static class HttpClientRegister
    {
        public static IServiceCollection HttpClientRegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClientService();
            string? HubtPath = configuration.GetSection("HUBT_Data").Get<string>();
            string? identityPath = configuration.GetSection("IdentityApi").Get<string>();
            string? cloudPath = configuration.GetSection("CloudServiceCenter").Get<string>();
            if (HubtPath != null)
                services.AddRegisterClientService<IOutSourceService, OutSourceService>(HubtPath);
            if (identityPath != null)
                services.AddRegisterClientService<IUserService, UserService>(identityPath);
            if (cloudPath != null)
                services.AddRegisterClientService<IHttpCloudService, HttpCloudService>(cloudPath);
            return services;
        }
    }
}
