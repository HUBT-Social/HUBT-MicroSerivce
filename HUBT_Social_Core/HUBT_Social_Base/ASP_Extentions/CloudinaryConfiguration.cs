using CloudinaryDotNet;
using HUBT_Social_Base.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Base.ASP_Extentions
{
    public static class CloudinaryConfiguration
    {
        public static IServiceCollection ConfigureCloudinary(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("Cloudinary");
            var account = new Account
            (
                config["CloudName"],
                config["ApiKey"],
                config["ApiSecret"]
            );
            var cloudinary = new Cloudinary(account);
            services.AddSingleton(cloudinary);
            

            return services;
        }
    }
}
