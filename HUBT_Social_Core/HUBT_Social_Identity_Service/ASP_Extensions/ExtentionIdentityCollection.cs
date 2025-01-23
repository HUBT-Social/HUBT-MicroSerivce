using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Identity_Service.ConstructorBuider;
using HUBT_Social_Identity_Service.Models;
using HUBT_Social_Identity_Service.Services;
using HUBT_Social_Identity_Service.Services.IdentityCustomeService;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace HUBT_Social_Identity_Service.ASP_Extensions
{
    public static class ExtentionIdentityCollection
    {
        public static ICustomIdentityBuilder AddTokenManager<TUser,TRole,TTokenCollection>(this ICustomIdentityBuilder builder)
            where TUser : MongoIdentityUser<Guid>, new()
            where TRole : MongoIdentityRole<Guid>, new()
            where TTokenCollection : IdentityToken, new()
        {
            builder.Services.RegisterMongoCollections(builder.DatabaseSetting, typeof(TTokenCollection));
            builder.Services.AddScoped<ITokenService<TUser,TTokenCollection>, TokenService<TUser, TTokenCollection>>();
            builder.Services.AddScoped<IHubtIdentityService<TUser, TRole, TTokenCollection>,
            HubtIdentityService<TUser, TRole, TTokenCollection>>();
            // Ghi đè đăng ký cũ với interface mới
            builder.Services.AddScoped<IHubtIdentityService<TUser, TRole>>(sp =>
                sp.GetRequiredService<IHubtIdentityService<TUser, TRole, TTokenCollection>>());

            builder.Services.MongoMapperConfiguration<TTokenCollection, TokenDTO>();
            return builder;
        }
    }
}
