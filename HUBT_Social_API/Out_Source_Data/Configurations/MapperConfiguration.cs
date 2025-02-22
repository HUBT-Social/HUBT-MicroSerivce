using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using Out_Source_Data.Src.Models;

namespace Out_Source_Data.Configurations
{
    public static class MapperConfiguration
    {
        public static IServiceCollection AddMongoMapper(this IServiceCollection services)
        {
            services.MongoMapperConfiguration<SinhVien, StudentDTO>();
            services.MongoMapperConfiguration<ThoiKhoaBieu, TimeTableDTO>();
            services.MongoMapperConfiguration<Diemtb, AVGScoreDTO>();
            return services;
        }
    }
}
