
using Chat_Data_API.Configurations;
using Chat_Data_API.Src.Hubs;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Chat_Service.ASP_Extensions;
using HUBT_Social_Chat_Service.Helper;
using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Chat_Data_API
{
    public class Program
    {

        private static void RegisterThirdPartyServices(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGenService();
            builder.Services.AddJwtConfiguration(builder.Configuration);
            builder.Services.AddMongoCollections(builder.Configuration);
            builder.Services.ConfigureLocalization();
            builder.Services.ConfigureCloudinary(builder.Configuration);
            builder.Services.HttpClientRegisterConfiguration(builder.Configuration);
            builder.Services.AddMongoMapper();
        }

        private static void RegisterApplicationServices(WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.RegistChatService();
            builder.Services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            // Cách khác nếu bạn cần cấu hình static TokenHelper
            // builder.Services.AddSingleton<TokenHelper>();
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            RegisterThirdPartyServices(builder);
            RegisterApplicationServices(builder);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetIsOriginAllowed(_ => true); // hoặc cụ thể domain mobile
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Nếu cần gọi TokenHelper.Configure
            var jwtOptions = app.Services.GetRequiredService<IOptions<JwtSetting>>();
            TokenHelper.Configure(jwtOptions);

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseLocalization();

            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}


