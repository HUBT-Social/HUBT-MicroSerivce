using HUBT_Social_Core.ASP_Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Microsoft.Extensions.Options;
using Gateway_API;
using Gateway_API.Hubs;

namespace Gateway_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);

            builder.Configuration.AddEnvironmentVariables();
            // Add services to the container.
            builder.Services.AddJwtConfiguration(builder.Configuration);

            // Read and replace placeholders in ocelot.swagger.json and router.json
            var routerJson = File.ReadAllText("router.json");
            var ocelotSwaggerJson = File.ReadAllText("ocelot.swagger.json");
            var services = builder.Configuration.GetSection("Services").GetChildren();

            foreach (var service in services)
            {
                string serviceName = service.Key;
                string hostVar = $"{{{serviceName}__Host}}";

                string? host = service.Value;

                if (!string.IsNullOrEmpty(host))
                {
                    ocelotSwaggerJson = ocelotSwaggerJson.Replace(hostVar, host);
                    routerJson = routerJson.Replace(hostVar, host);
                }
            }

            // Save the modified configuration back to the file or use it directly
            var tempOcelotPath = Path.GetTempFileName();
            var tempRouterPath = Path.GetTempFileName();

            File.WriteAllText(tempOcelotPath, ocelotSwaggerJson);
            File.WriteAllText(tempRouterPath, routerJson);

            builder.Configuration.AddJsonFile(tempOcelotPath, optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile(tempRouterPath, optional: false, reloadOnChange: true);

            // Add controllers
            builder.Services.AddControllers();

            // Configure Swagger for both Gateway and downstream services
            builder.Services.AddEndpointsApiExplorer();

            // Add standard Swagger for your Gateway API
            builder.Services.AddSwaggerGen(op =>
            {
                op.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            // Add Swagger for Ocelot
            builder.Services.AddSwaggerForOcelot(builder.Configuration, option =>
            {
                option.GenerateDocsForGatewayItSelf = true;
            });

            // Add Ocelot services
            builder.Services.AddOcelot(builder.Configuration);

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

            builder.Services.AddSignalR();

            var app = builder.Build();
            app.UseCors("AllowReactApp");

            // Configure the HTTP request pipeline.
            app.UseSwagger();

            app.UseSwaggerForOcelotUI(options =>
            {
                options.PathToSwaggerGenerator = "/swagger/docs";
            });


            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseRouting();

            // Use top-level route registrations instead of UseEndpoints
            app.MapControllers();
            app.MapHub<SpinUpHub>("/statusHub");

            // Use Ocelot only once
            app.UseWebSockets();
            app.UseWhen(context =>
                !(context.Request.Path == "/" || context.Request.Path.StartsWithSegments("/statusHub")),
                appBuilder =>
                {
                    appBuilder.UseOcelot().Wait();
                });
                
            app.Run();
        }
    }
}
