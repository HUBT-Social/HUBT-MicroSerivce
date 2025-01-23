using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_Core.Settings;
using HUBT_Social_Identity_Service.Configurations;
using HUBT_Social_Identity_Service.ConstructorBuider;
using HUBT_Social_Identity_Service.Services;
using HUBT_Social_Identity_Service.Services.IdentityCustomeService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace HUBT_Social_Identity_Service.ASP_Extensions
{
    public static class IdentityConfiguration
    {
        public static ICustomIdentityBuilder AddCustomIdentity<TUser, TRole>(
           this IServiceCollection services,
           DatabaseSetting database
        )
       where TUser : MongoIdentityUser<Guid>, new()
       where TRole : MongoIdentityRole<Guid>, new()
        {
            MongoDbIdentityConfiguration mongodbConfig = new()
            {
                MongoDbSettings = new MongoDbSettings
                {
                    ConnectionString = database.ConnectionString,
                    DatabaseName = database.DatabaseName
                },
                IdentityOptionsAction = option =>
                {
                    option.Password.RequireDigit = true;
                    option.Password.RequiredLength = 8;
                    option.Password.RequireNonAlphanumeric = false;
                    option.Password.RequireLowercase = true;
                    option.Password.RequireUppercase = true;
                    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    option.Lockout.MaxFailedAccessAttempts = 5;
                    option.User.RequireUniqueEmail = true;
                }
            };

            services.ConfigureMongoDbIdentity<TUser, TRole, Guid>(mongodbConfig)
                .AddUserManager<UserManager<TUser>>()
                .AddSignInManager<SignInManager<TUser>>()
                .AddRoleManager<RoleManager<TRole>>()
                .AddDefaultTokenProviders();
            
            services.AddScoped<IUserService<TUser, TRole>, UserService<TUser, TRole>>();
            services.AddScoped<IAuthenService<TUser, TRole>, AuthenService<TUser, TRole>>();
            services.AddScoped<IHubtIdentityService<TUser, TRole>, HubtIdentityService<TUser, TRole>>();

            return new CustomIdentityBuilder(services, database);
        }
    }
}
