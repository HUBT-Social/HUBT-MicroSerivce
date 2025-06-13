
using HUBT_Social_Core.Settings;
using HUBT_Social_Email_Service.ASP_Extentions;

namespace Notation_API.Configruations
{
    public static class EmailServiceConfiguration
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            SMPTSetting? setting = configuration.GetSection("SMPTSetting").Get<SMPTSetting>();
            if (setting != null)
            {
                services.InitEmailService(setting).InitEmailNotificationService();
                return services;
            }
            throw new Exception("Unable to connect SMPT service");
        }
    }
}
