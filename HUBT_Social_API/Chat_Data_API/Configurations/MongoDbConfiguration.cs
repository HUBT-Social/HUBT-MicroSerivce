using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using MongoDB.Driver;

namespace Chat_Data_API.Configurations
{
    public static class MongoDbConfiguration
    {
        public static IServiceCollection AddMongoCollections(this IServiceCollection services,
        IConfiguration configuration)
        {
            DatabaseSetting? connectionstring = configuration.GetSection("ChatDB").Get<DatabaseSetting>();

            if (connectionstring != null)
            {
                services.RegisterMongoCollections(connectionstring, typeof(ChatGroupModel));
                return services;
            }
            throw new Exception("Unable to genarate Mongodb");

        }
    }
}
