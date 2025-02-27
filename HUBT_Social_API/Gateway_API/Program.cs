using HUBT_Social_Core.ASP_Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Gateway_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddJwtConfiguration(builder.Configuration);
            builder.Configuration.AddJsonFile("router.json", false, true);
            builder.Configuration.AddJsonFile("ocelot.swagger.json", false, true);

            builder.Services.AddSwaggerForOcelot(builder.Configuration);
            builder.Services.AddOcelot(builder.Configuration);
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGenService();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerForOcelotUI(options =>
                {
                    options.PathToSwaggerGenerator = "/swagger/docs";
                }).UseOcelot().Wait();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            await app.UseOcelot();
            app.Run();
        }
    }
}

