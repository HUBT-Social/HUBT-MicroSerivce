using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using TempRegister_API.Src.Models;

namespace TempRegister_API.Configurations
{
    public static class MapperConfiguration
    {
        public static IServiceCollection AddMongoMapper(this IServiceCollection services)
        {
            services.MongoMapperConfiguration<TempUserRegister,TempUserDTO>();
            services.MongoMapperConfiguration<TempCourse, TempCourse>();
            return services;
        }
    }
}
