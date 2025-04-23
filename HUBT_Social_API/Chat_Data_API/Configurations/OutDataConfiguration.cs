
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using Chat_Data_API.Src.Models;
using Chat_Data_API.Src.Model;

namespace Identity_API.Configurations;
public static class OutDataConfiguration
{
    public static IServiceCollection AddOutDataConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        DatabaseSetting? databaseSetting = configuration.GetSection("OutData").Get<DatabaseSetting>();

        if (databaseSetting != null)
        {      
            services.RegisterMongoCollections(databaseSetting,typeof(SinhVien), typeof(ThoiKhoaBieu),typeof(Course));
            return services;
        }
        Console.WriteLine("Unable to connect to OutData");
        return services;
    }
}