using HUBT_Social_Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace HUBT_Social_Identity_Service.ConstructorBuider
{
    public class CustomIdentityBuilder(IServiceCollection services, DatabaseSetting databaseSetting) : ICustomIdentityBuilder
    {
        public IServiceCollection Services { get; } = services;
        public DatabaseSetting DatabaseSetting { get; } = databaseSetting;

    }
}
