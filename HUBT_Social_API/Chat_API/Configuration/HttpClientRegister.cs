using Chat_API.Src.Interfaces;
using Chat_API.Src.Services;
using HUBT_Social_Base.ASP_Extentions;

namespace Chat_API.Configuration
{
    public static class HttpClientRegister
    {
        public static IServiceCollection HttpClientRegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClientService();
            string? chatPath = configuration.GetSection("ChatApi").Get<string>();
            string? userPath = configuration.GetSection("UserAPI").Get<string>();
            if (chatPath != null || userPath !=null)
            {
                services.AddRegisterClientService<IChatService,ChatService>(chatPath);
                services.AddRegisterClientService<IRoomService, RoomService>(chatPath);
                services.AddRegisterClientService<IUserService, UserService>(userPath);
            }

            return services;
        }
    }
}
