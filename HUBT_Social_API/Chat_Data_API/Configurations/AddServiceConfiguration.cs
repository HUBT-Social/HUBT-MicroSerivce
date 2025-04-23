using Chat_Data_API.Src.Service;

namespace Chat_Data_API.Configurations
{
    public static class AddServiceConfiguration
    {
        public static IServiceCollection OutServiceRegister(this IServiceCollection services)
        {
            services.AddScoped<IOutDataService, OutDataService>();
            return services;
        }
    }
}
