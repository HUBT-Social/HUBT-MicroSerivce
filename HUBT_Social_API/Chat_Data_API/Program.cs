
using Chat_Data_API.Configurations;
using Chat_Data_API.Src.Hubs;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Chat_Service.ASP_Extensions;
using HUBT_Social_Chat_Service.Helper;
using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Core.Settings;
using Identity_API.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Chat_Data_API
{
    public class Program
    {

        private static void InitConfigures(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGenService();
            builder.Services.AddJwtConfiguration(builder.Configuration);
            builder.Services.AddMongoCollections(builder.Configuration);
            builder.Services.ConfigureLocalization();
            builder.Services.ConfigureCloudinary(builder.Configuration);
            builder.Services.AddMongoMapper();
            builder.Services.AddOutDataConfiguration(builder.Configuration);
            builder.Services.OutServiceRegister();



        }
        private static void InitServices(WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtSettings"));
            TokenHelper.Configure(builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JwtSetting>>());
            builder.Services.RegistChatService();
            builder.Services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
            builder.Services.AddControllers();
            builder.Services.AddSignalR();
        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            InitConfigures(builder);
            InitServices(builder);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("https://chatuitest.onrender.com", "http://localhost:3000")  // Chỉ cho phép origin này
                        .AllowAnyMethod()   // Cho phép bất kỳ phương thức HTTP nào
                        .AllowAnyHeader()   // Cho phép bất kỳ header nào
                        .AllowCredentials(); // Cho phép gửi credentials như cookies, authorization headers
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseCors("AllowReactApp");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseLocalization();
            
            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");
            
            app.Run();
        }
    }
}


