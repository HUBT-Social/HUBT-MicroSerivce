using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Firebase.ASP_Extensions;
using Notation_API.Configurations;
using System;
using Notation_API.Src.Repository;
using MongoDB.Driver;
using HUBT_Social_Core.Settings;
using Notation_API.Configruations;

namespace Notation_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // ===== Configure Services =====
                ConfigureCoreServices(builder);
                ConfigureHangfire(builder);
                ConfigureCors(builder);

                var app = builder.Build();

                // ===== Configure Middleware =====
                ConfigureMiddleware(app);
                RegisterHangfireJobs(app);

                Console.WriteLine("✅ Application started successfully!");
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Application failed to start: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private static void ConfigureCoreServices(WebApplicationBuilder builder)
        {
            try
            {
                builder.Services.AddControllers();
                builder.Services.AddAuthorization();
                builder.Services.AddEndpointsApiExplorer();

                // Tạm thời dùng default Swagger để test
                try
                {
                    builder.Services.AddSwaggerGenService(); ;
                    Console.WriteLine("✅ Default Swagger added successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error adding Swagger: {ex.Message}");
                    // Fallback to custom if default fails
                    builder.Services.AddSwaggerGenService();
                }
                builder.Services.ConfigureLocalization(); // Localization (tuỳ chỉnh trong extension)
                builder.Services.HttpClientRegisterConfiguration(builder.Configuration); // HttpClient
                builder.Services.AddEmailService(builder.Configuration);    
                builder.Services.AddJwtConfiguration(builder.Configuration); // JWT Auth
                builder.Services.FirebaseService(builder.Configuration); // Firebase (FCM)
                builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtSettings"));

                ConfigureMongoDB(builder);

                Console.WriteLine("✅ Core services configured successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error configuring core services: {ex.Message}");
                throw;
            }
        }

        private static void ConfigureHangfire(WebApplicationBuilder builder)
        {
            try
            {
                var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDbHangfireProduction");

                builder.Services.AddHangfire(config => config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseMongoStorage(mongoConnectionString, new MongoStorageOptions
                    {
                        MigrationOptions = new MongoMigrationOptions
                        {
                            MigrationStrategy = new MigrateMongoMigrationStrategy(),
                            BackupStrategy = new CollectionMongoBackupStrategy()
                        },
                        // Fix: Use polling instead of Change Streams
                        CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                    }));

                builder.Services.AddHangfireServer();
                Console.WriteLine("✅ Hangfire configured successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error configuring Hangfire: {ex.Message}");
                throw;
            }
        }
        private static void ConfigureMongoDB(WebApplicationBuilder builder)
        {
            try
            {
                var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDBNotitonProduction");


                // Register MongoClient và IMongoDatabase
                builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
                {
                    return new MongoClient(mongoConnectionString);
                });

                builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
                {
                    var client = serviceProvider.GetRequiredService<IMongoClient>();
                    var databaseName = MongoUrl.Create(mongoConnectionString).DatabaseName ?? "notation_db";
                    return client.GetDatabase(databaseName);
                });

                // Register repositories
                builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

                Console.WriteLine("✅ MongoDB configured successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error configuring MongoDB: {ex.Message}");
                throw;
            }
        }

        private static void ConfigureCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins(
                        "https://chatuitest.onrender.com",
                        "https://hubt-social-web.onrender.com",
                        "http://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            try
            {
                app.UseRouting();

                // Exception handling
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/error");
                    app.UseHsts();
                }

                // Swagger - tạm thời disable để test
                if (app.Environment.IsDevelopment())
                {
                    try
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI();
                        Console.WriteLine("✅ Swagger configured successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error configuring Swagger: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                        // Tạm thời comment để app vẫn chạy được
                        // throw;
                    }
                }

                // Middlewares
                app.UseHttpsRedirection();
                app.UseCors("AllowReactApp");
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseLocalization();

                // Hangfire Dashboard
                app.UseHangfireDashboard("/hangfire");

                app.MapControllers();
                Console.WriteLine("✅ Middleware configured successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error configuring middleware: {ex.Message}");
                throw;
            }
        }

        private static void RegisterHangfireJobs(WebApplication app)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var backgroundJobClient = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();

                // Test Job
                backgroundJobClient.Enqueue(() => Console.WriteLine("🔥 Hangfire MongoDB đã hoạt động!"));
              
                Console.WriteLine("✅ Hangfire jobs registered successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error registering Hangfire jobs: {ex.Message}");
                throw;
            }
        }
    }
}