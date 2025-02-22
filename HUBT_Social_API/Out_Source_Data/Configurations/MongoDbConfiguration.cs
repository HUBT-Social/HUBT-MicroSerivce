using MongoDB.Driver;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_Core.Settings;
using Out_Source_Data.Src.Models;

namespace Out_Source_Data.Configurations;

public static class MongoDbConfiguration
{
    public static IServiceCollection AddMongoCollections(this IServiceCollection services,
        IConfiguration configuration)
    {
        DatabaseSetting? connectionstring = configuration.GetSection("HubtDataBase").Get<DatabaseSetting>();

        if (connectionstring != null)
        {
            services.RegisterMongoCollections(connectionstring, typeof(Diemtb)
                , typeof(SinhVien)
                , typeof(ThoiKhoaBieu));
            return services;
        }
        throw new Exception("Unable to genarate Mongodb");

    }

}