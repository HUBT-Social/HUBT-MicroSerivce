using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using TempRegister_API.Src.Service;

namespace User_API.Configurations
{
    public static class HttpClientRegister
    {
        public static IServiceCollection HttpClientRegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClientService();
            string pathOut = configuration.GetSection("OutService").Get<string>() ?? "";
            string pathNotifi = configuration.GetSection("NotifiService").Get<string>() ?? "";

            if (pathOut != null)
                services.AddRegisterClientService<IOutService, OutService>(pathOut);
            if (pathNotifi != null)
                services.AddRegisterClientService<INotifiService, NotifiService>(pathNotifi);

            return services;
        }
    }
}
