using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Firebase.ASP_Extensions;
using Notation_API.Configurations;
namespace Notation_API
{
    public class Program
    {
        private static void InitConfigures(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGenService();
            builder.Services.ConfigureLocalization();
            builder.Services.HttpClientRegisterConfiguration(builder.Configuration);
            builder.Services.AddJwtConfiguration(builder.Configuration);
            builder.Services.FirebaseService(builder.Configuration);
        }
        private static void InitServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
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
                    policy.WithOrigins("https://chatuitest.onrender.com", "https://hubt-social-web.onrender.com", "http://localhost:5173")  // Chỉ cho phép origin này
                        .AllowAnyMethod()   // Cho phép bất kỳ phương thức HTTP nào
                        .AllowAnyHeader()   // Cho phép bất kỳ header nào
                        .AllowCredentials(); // Cho phép gửi credentials như cookies, authorization headers
                });
            });

            var app = builder.Build();

            app.UseCors("AllowReactApp");
            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseLocalization();

            app.MapControllers();
        
            app.Run();
        }
    }
}
