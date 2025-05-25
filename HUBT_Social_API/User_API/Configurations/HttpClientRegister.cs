using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using User_API.Src.Service;

namespace User_API.Configurations
{
    public static class HttpClientRegister
    {
        public static IServiceCollection HttpClientRegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClientService();
            string? identityPath = configuration.GetSection("IdentityApi").Get<string>();
            string? NotationPath = configuration.GetSection("NotationApi").Get<string>();
            string? HubtPath = configuration.GetSection("HUBT_Data").Get<string>();
            string? tempUserPath = configuration.GetSection("TempUserApi").Get<string>();
            string? chatPath = configuration.GetSection("ChatApi").Get<string>();
            string? cloudPath = configuration.GetSection("CloudServiceCenter").Get<string>();

            if (identityPath != null)
                services.AddRegisterClientService<IUserService, UserService>(identityPath);
            
            if (NotationPath != null)
                services.AddRegisterClientService<INotationService, NotationService>(NotationPath);
            
            if (HubtPath != null)
                services.AddRegisterClientService<IOutSourceService, OutSourceService>(HubtPath);
            
            if (tempUserPath != null)
                services.AddRegisterClientService<ITempService, TempService>(tempUserPath);
            if (chatPath != null)
                services.AddRegisterClientService<IChatService,ChatService>(chatPath);
            if (cloudPath != null)
                services.AddRegisterClientService<IHttpCloudService, HttpCloudService>(cloudPath);
            return services;
        }
    }
}
