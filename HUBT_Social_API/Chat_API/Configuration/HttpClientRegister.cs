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
            string? chatPath = configuration.GetSection("ChatApi").Get<string>() ?? string.Empty;
            string? userPath = configuration.GetSection("UserAPI").Get<string>() ?? string.Empty;
            string? outDatPath = configuration.GetSection("OutSourceData").Get<string>() ?? string.Empty;
            if (chatPath != null)
            {
                services.AddRegisterClientService<IChatService,ChatService>(chatPath);
                services.AddRegisterClientService<IRoomService, RoomService>(chatPath);
            }
            if (userPath != null)
            {
                services.AddRegisterClientService<IUserService, UserService>(userPath);
            }
            if (outDatPath != null)
            {
                services.AddRegisterClientService<IUserService, UserService>(outDatPath);
            }

            return services;
        }
    }
}
