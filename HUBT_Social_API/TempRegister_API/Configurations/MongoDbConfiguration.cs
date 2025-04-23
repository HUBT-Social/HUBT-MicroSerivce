using TempRegister_API.Src.Models;
using MongoDB.Driver;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.Models.OutSourceDataDTO;

namespace TempRegister_API.Configurations;

public static class MongoDbConfiguration
{
    public static IServiceCollection AddMongoCollections(this IServiceCollection services,
        IConfiguration configuration)
    {
        DatabaseSetting? connectionstring = configuration.GetSection("TempRegisterDB").Get<DatabaseSetting>();

        if (connectionstring != null)
        {
            services.RegisterMongoCollections(connectionstring, typeof(TempUserRegister));
            services.RegisterMongoCollections(connectionstring, typeof(TempCourse));
            return services;
        }
        throw new Exception("Unable to genarate Mongodb");

    }

}