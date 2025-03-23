using Chat_Data_API.Src.Service;
using HUBT_Social_Base.ASP_Extentions;

namespace Chat_Data_API.Configurations
{
    public static class HttpClientRegister
    {
        public static IServiceCollection HttpClientRegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClientService();
            string? identityPath = configuration.GetSection("Notition").Get<string>();
            if (identityPath != null)
                services.AddRegisterClientService<INotition, Notition>(identityPath);
            return services;
        }
    }
}
