using AspNetCore.Identity.MongoDbCore.Models;
using AutoMapper;
using HUBT_Social_Identity_Service.Configurations;
using Microsoft.Extensions.DependencyInjection;


namespace HUBT_Social_Identity_Service.ASP_Extensions
{
    public static class MapperConfiguration
    {
        public static IServiceCollection AddCustomIdentityMapper<TUser, TRole>(
            this IServiceCollection services
        )
            where TUser : MongoIdentityUser<Guid>, new()
            where TRole : MongoIdentityRole<Guid>, new()
        {
            var mappingConfig = new AutoMapper.MapperConfiguration(mc =>
            {
                mc.AddProfile(new IdentityMapper<TUser, TRole>());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }
    }
}
