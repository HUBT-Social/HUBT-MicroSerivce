using HUBT_Social_Core.ASP_Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Gateway_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();
            // Add services to the container.
            builder.Services.AddJwtConfiguration(builder.Configuration);
            builder.Configuration.AddJsonFile("router.json", false, true);

            // Read and replace placeholders in ocelot.swagger.json
            var ocelotSwaggerJson = File.ReadAllText("ocelot.swagger.json");
            ocelotSwaggerJson = ocelotSwaggerJson
                .Replace("${AuthService__Host}", Environment.GetEnvironmentVariable("AuthService__Host"))
                .Replace("${AuthService__Port}", Environment.GetEnvironmentVariable("AuthService__Port"))
                .Replace("${UserService__Host}", Environment.GetEnvironmentVariable("UserService__Host"))
                .Replace("${UserService__Port}", Environment.GetEnvironmentVariable("UserService__Port"))
                .Replace("${NotationService__Host}", Environment.GetEnvironmentVariable("NotationService__Host"))
                .Replace("${NotationService__Port}", Environment.GetEnvironmentVariable("NotationService__Port"));

            // Save the modified configuration back to the file or use it directly
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, ocelotSwaggerJson);
            builder.Configuration.AddJsonFile(tempFilePath, optional: false, reloadOnChange: true);

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
