using HUBT_Social_Base.ASP_Extentions;
using Notation_API.Src.Services;

namespace Notation_API.Configurations
{
    public static class HttpClientRegister
    {
        public static IServiceCollection HttpClientRegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClientService();
            string? identityPath = configuration.GetSection("HUBT_Data").Get<string>();
            if (identityPath != null)
            {
                services.AddRegisterClientService<INotationService, NotationService>(identityPath);
                return services;
            }
            throw new Exception("Unfound Section");
        }
    }
}
