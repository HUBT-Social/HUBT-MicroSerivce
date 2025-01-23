using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_Core.Settings;
using Postcode_API.Src.Models;

namespace Postcode_API.Configruations;

public static class MongoDbConfiguration
{
    public static IServiceCollection AddMongoCollections(this IServiceCollection services,
        IConfiguration configuration)
    {
        DatabaseSetting? connectionstring = configuration.GetSection("PostcodeDB").Get<DatabaseSetting>();

        if (connectionstring != null)
        {
            services.RegisterMongoCollections(connectionstring, typeof(Postcode));
            return services;
        }
        throw new Exception("Unable to genarate Mongodb");

    }

}