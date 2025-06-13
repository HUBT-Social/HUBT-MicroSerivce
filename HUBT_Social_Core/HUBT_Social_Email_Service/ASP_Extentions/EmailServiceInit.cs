using HUBT_Social_Core.Settings;
using HUBT_Social_Email_Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Email_Service.ASP_Extentions
{
    public static class EmailServiceInit
    {
        public static IServiceCollection InitEmailService(this IServiceCollection services, SMPTSetting setting)
        {
            services.Configure<SMPTSetting>(options =>
            {
                options.Host = setting.Host;
                options.Port = setting.Port;
                options.Email = setting.Email;
                options.Password = setting.Password;
            });

            return services;
        }
        public static IServiceCollection InitEmailPostCodeService(this IServiceCollection service)
        {
            service.AddScoped<IEmailPostCodeService, EmailPostCodeService>();
            return service;
        }
        public static IServiceCollection InitEmailNotificationService(this IServiceCollection service)
        {
            service.AddScoped<IEmailNotification, EmailNotification>();
            return service;
        }

    }
}
