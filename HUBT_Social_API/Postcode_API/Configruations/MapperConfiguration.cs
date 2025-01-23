using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using Postcode_API.Src.Models;

namespace Postcode_API.Configruations
{
    public static class MapperConfiguration
    {
        public static IServiceCollection AddMongoMapper(this IServiceCollection services)
        {
            services.MongoMapperConfiguration<Postcode, PostCodeDTO>();
            return services;
        }
    }
}
