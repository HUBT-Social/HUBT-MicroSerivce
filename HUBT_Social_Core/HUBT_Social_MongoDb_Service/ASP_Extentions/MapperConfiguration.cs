
using AutoMapper;
using HUBT_Social_MongoDb_Service.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace HUBT_Social_MongoDb_Service.ASP_Extentions
{
    public static class MapperConfiguration
    {
        private static readonly List<Profile> Profiles = [];
        public static IServiceCollection MongoMapperConfiguration<TCollection, TCollectionDTO>(
            this IServiceCollection services
        )
            where TCollection :class,new()
            where TCollectionDTO : class, new()
        {
            Profiles.Add(new MongoMapper<TCollection, TCollectionDTO>());

            var mappingConfig = new AutoMapper.MapperConfiguration(mc =>
            {
                foreach (var profile in Profiles)
                {
                    mc.AddProfile(profile);
                }
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }
    }
}
