using HUBT_Social_Base.Service;
using Microsoft.Extensions.DependencyInjection;

namespace HUBT_Social_Base.ASP_Extentions
{
    public static class HttpClientConfiguration
    {
        public static IServiceCollection AddRegisterClientService<IService, Service>(
            this IServiceCollection services,
            string basePath
        )
        where IService : class, IBaseService
        where Service : class, IService
        {
            services.AddHttpClient<IService, Service>();
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<IService>(sp =>
            {
                var httpService = sp.GetRequiredService<IHttpService>();
                return ActivatorUtilities.CreateInstance<Service>(sp, httpService, basePath);
            });
            return services;
        }
        public static IServiceCollection AddHttpClientService(
            this IServiceCollection services
        )
        {
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddScoped<IHttpService, HttpService>();
            return services;
        }
    }
}
