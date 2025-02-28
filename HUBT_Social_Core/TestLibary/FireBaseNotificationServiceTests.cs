using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Core.Settings;
using HUBT_Social_Firebase.ASP_Extensions;
using HUBT_Social_Firebase.Services;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using TestLibary.Service;
using Xunit;

namespace TestLibary
{
    public class FireBaseNotificationServiceTests
    {
        private readonly IConfiguration _configuration;
        private readonly ServiceCollection _service;

        public FireBaseNotificationServiceTests()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _service = new ServiceCollection();
            _service.FirebaseService(_configuration);
            _service.AddHttpClientService();
            string? identityPath = _configuration.GetSection("HUBT_Data").Get<string>() ?? throw new Exception("Unfound Section");
            _service.AddRegisterClientService<INotationService, NotationService>(identityPath);
            
        }
       
        [Fact]
        public async void FirebaseService_IFirebaseNotification_SendNotification()
        {
            // Arrange
            var serviceProvider = _service.BuildServiceProvider();
            IFireBaseNotificationService? firebaseService = serviceProvider.GetService<IFireBaseNotificationService>();
            SendMessageRequest sendMessageRequest = new()
            {
                Token = "cWEP5KXJRpepYwrROc1kJE:APA91bECrrfG_qDe9Sldw_3PAq3iDO0Ns4KNJ3ak3b4EmeY1jal6-sRkMypBMIz7AqP80mFE1OsG3dNNRIaQe4xAScF1bAs1JAP3b-GDJzFhVEp6_gKknUM",
                Title = "ToDuong",
                Body = "Hello"
            };
            //Assert
            Assert.NotNull(firebaseService);
            Assert.IsType<FireBaseNotificationService>(firebaseService);
            // Act & Assert dadad
            var exception = await Record.ExceptionAsync(async () => await firebaseService.SendPushNotificationAsync(sendMessageRequest));
            Assert.Null(exception);
        }

    }
}