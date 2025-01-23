using HUBT_Social_Core.Settings;
using HUBT_Social_Email_Service.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Email_Service.ASP_Extentions
{
    public static class EmailServiceInit
    {
        public static IServiceCollection InitEmailService(this IServiceCollection service, SMPTSetting setting)
        {
            //service.Configure<SMPTSetting>(options =>
            //{
            //    options.Host = setting.Host; 
            //    options.Port = setting.Port; 
            //    options.Password = setting.Password;
            //    options.Email = setting.Email;
            //});
            service.AddSingleton(setting);
            service.AddScoped<IEmailService, EmailService>();
            return service;
        }
    }
}
