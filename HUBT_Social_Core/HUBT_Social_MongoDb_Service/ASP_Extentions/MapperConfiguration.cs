
using AutoMapper;
using HUBT_Social_MongoDb_Service.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace HUBT_Social_MongoDb_Service.ASP_Extentions
{
    public static class MapperConfiguration
    {
        public static IServiceCollection MongoMapperConfiguration<TCollection, TCollectionDTO>(
            this IServiceCollection services
        )
            where TCollection :class,new()
            where TCollectionDTO : class, new()
        {
            var mappingConfig = new AutoMapper.MapperConfiguration(mc =>
            {
                mc.AddProfile(new MongoMapper<TCollection, TCollectionDTO>());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }
    }
}
