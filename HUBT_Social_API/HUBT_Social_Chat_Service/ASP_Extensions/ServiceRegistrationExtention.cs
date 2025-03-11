using HUBT_Social_Base.Service;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_Chat_Service.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Service.ASP_Extensions
{
    public static class ServiceRegistrationExtention
    {
        public static IServiceCollection RegistChatService(this IServiceCollection service)
        {
            service.AddScoped<IMessageUploadService, MessageUploadService>(); 
            service.AddScoped<IMediaUploadService, MediaUploadService>();
            service.AddScoped<IUploadService, UploadService>();
            service.AddScoped<IChatService, ChatService>();
            service.AddScoped<IRoomUpdateService, RoomUpdateService>();
            service.AddScoped<IRoomGetService, RoomGetService>();
            service.AddScoped<ICloudService, CloudService>();


            return service;
        }

    }
}
