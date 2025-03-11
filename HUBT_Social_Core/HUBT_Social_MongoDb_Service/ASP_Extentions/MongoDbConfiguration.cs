using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Reflection;

namespace HUBT_Social_MongoDb_Service.ASP_Extentions;

public static class MongoDbConfiguration
{
    public static IServiceCollection RegisterMongoCollections(
        this IServiceCollection services,
        DatabaseSetting dbSetting,
        params Type[] collections)
        
    {
        // Initialize MongoDB client and database
        MongoClient client = new(dbSetting.ConnectionString);
        var database = client.GetDatabase(dbSetting.DatabaseName);

        var collectio = database.ListCollectionNames().ToList();
        Console.WriteLine($"Collections in database: {string.Join(", ", collectio)}");

        foreach (Type collectionType in collections)
        {

            MethodInfo? method = typeof(IMongoDatabase)
                .GetMethod(nameof(IMongoDatabase.GetCollection))
                ?.MakeGenericMethod(collectionType);

            if (method != null)
            {
                object? collection = method.Invoke(database, [collectionType.Name, null]);
                if (collection != null)
                {
                    var mongoCollectionType = typeof(IMongoCollection<>).MakeGenericType(collectionType);
                    services.AddScoped(mongoCollectionType, _ => collection);

                    var serviceType = typeof(IMongoService<>).MakeGenericType(collectionType);
                    var implementationType = typeof(MongoService<>).MakeGenericType(collectionType);
                    services.AddScoped(serviceType, implementationType);

                }

            }
        }

        return services;
    }

}