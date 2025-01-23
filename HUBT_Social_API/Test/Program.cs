
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Identity_API.Src.Models;
using Microsoft.AspNetCore.Identity;
using Test.Models;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            MongoDbIdentityConfiguration mongodbConfig = new()
            {
                MongoDbSettings = new MongoDbSettings
                {
                    ConnectionString = "mongodb+srv://duonghb:Duonghb123@mongoc.tzz8o.mongodb.net/",
                    DatabaseName = "DatabaseName"
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
            builder.Services.AddSingleton(TimeProvider.System);

            // Thêm Data Protection
            builder.Services.AddDataProtection();

            builder.Services.ConfigureMongoDbIdentity<AUser, ARole, Guid>(mongodbConfig)
            .AddUserManager<UserManager<AUser>>()
            .AddSignInManager<SignInManager<AUser>>()
            .AddRoleManager<RoleManager<ARole>>()
            .AddDefaultTokenProviders();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
