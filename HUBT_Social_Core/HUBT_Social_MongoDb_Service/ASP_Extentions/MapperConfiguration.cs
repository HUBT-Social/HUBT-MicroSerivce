
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
            return ConfigureMapper(services);
        }
      

       
        public static IServiceCollection AddCustomMappingProfile<TProfile>(
            this IServiceCollection services
        )
            where TProfile : Profile, new()
        {
            Profiles.Add(new TProfile());
            return ConfigureMapper(services);
        }

        private static IServiceCollection ConfigureMapper(IServiceCollection services)
        {
            var mappingConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                foreach (var profile in Profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }
    }
}

