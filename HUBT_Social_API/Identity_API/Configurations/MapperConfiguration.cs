using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Identity_Service.ASP_Extensions;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using Identity_API.Src.Models;

namespace Identity_API.Configurations
{
    public static class MapperConfiguration
    {
        public static IServiceCollection AddMongoMapper(this IServiceCollection services)
        {
            services.MongoMapperConfiguration<UserToken, TokenDTO>();
            services.AddCustomIdentityMapper<AUser,ARole>();
            return services;
        }
    }
}
