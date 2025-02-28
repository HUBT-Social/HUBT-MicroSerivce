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

            // Add controllers
            builder.Services.AddControllers();

            // Configure Swagger for both Gateway and downstream services
            builder.Services.AddEndpointsApiExplorer();

            // Add standard Swagger for your Gateway API
            builder.Services.AddSwaggerGenService(op =>
            {
                op.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            // Add Swagger for Ocelot
            builder.Services.AddSwaggerForOcelot(builder.Configuration);

            // Add Ocelot services
            builder.Services.AddOcelot(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    // First register regular Swagger for your Gateway API
            //    app.UseSwagger();

            //    // Then register SwaggerForOcelot for downstream services
            //    // This creates a separate UI at /swagger/docs
            //    app.UseSwaggerForOcelotUI(options =>
            //    {
            //        options.PathToSwaggerGenerator = "/swagger/docs";
            //    });
            //}
            app.UseSwagger();

            app.UseSwaggerForOcelotUI(options =>
            {
                options.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Use Ocelot only once
            await app.UseOcelot();

            app.Run();
        }
    }
}