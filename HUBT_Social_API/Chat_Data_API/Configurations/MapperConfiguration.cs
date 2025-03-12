using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_MongoDb_Service.ASP_Extentions;

namespace Chat_Data_API.Configurations
{
    public static class MapperConfiguration
    {
        public static IServiceCollection AddMongoMapper(this IServiceCollection services)
        {
            services.MongoMapperConfiguration<ChatGroupModel, ChatGroupModel>();
            return services;
        }
    }
}
