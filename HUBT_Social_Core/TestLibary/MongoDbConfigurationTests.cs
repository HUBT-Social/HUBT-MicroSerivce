using Moq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_MongoDb_Service.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace TestLibary
{
    public class MongoDbConfigurationTests
    {
        private readonly DatabaseSetting _dbSetting;
        private readonly ServiceCollection _service;
        public MongoDbConfigurationTests()
        {
            _dbSetting = new DatabaseSetting
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "TestDatabase"
            };
            _service = new ServiceCollection();
            _service.RegisterMongoCollections(_dbSetting, typeof(TestCollection));
        }

        [Fact]
        public void RegisterMongoCollections_ShouldRegisterServices()
        {


            // Assert
            var serviceProvider = _service.BuildServiceProvider();
            var collection = serviceProvider.GetService<IMongoCollection<TestCollection>>();
            var mongoService = serviceProvider.GetService<IMongoService<TestCollection>>();

            Assert.NotNull(collection);
            Assert.NotNull(mongoService);
        }

        [Fact]
        public async Task ResolveService_ShouldUseIMongoService()
        {
            // Arrange
            var serviceProvider = _service.BuildServiceProvider();
            var mongoService = serviceProvider.GetService<IMongoService<TestCollection>>();

            // Ensure mongoService is not null
            Assert.NotNull(mongoService);

            // Act
            var testCollection = new TestCollection
            {
                Email = "test@example.com",
                UserName = "testuser",
                Password = "Password123",
                ExpireTime = DateTime.UtcNow
            };

            await mongoService!.Create(testCollection);
            var retrievedCollection = await mongoService.GetById(testCollection.Email);

            // Assert
            Assert.NotNull(retrievedCollection);
            Assert.Equal(testCollection.Email, retrievedCollection?.Email);
            await mongoService.Delete(testCollection);
            Assert.Null(await mongoService.GetById(testCollection.Email));
        }

        [Fact]
        public async Task DeleteService_ShouldRemoveDocument()
        {
            // Arrange

            var serviceProvider = _service.BuildServiceProvider();
            var mongoService = serviceProvider.GetService<IMongoService<TestCollection>>();

            // Ensure mongoService is not null
            Assert.NotNull(mongoService);

            // Act
            var testCollection = new TestCollection
            {
                Email = "test@example.com",
                UserName = "testuser",
                Password = "Password123",
                ExpireTime = DateTime.UtcNow
            };

            await mongoService!.Create(testCollection);
            var deleteResult = await mongoService.Delete(testCollection);
            var retrievedCollection = await mongoService.GetById(testCollection.Email);

            // Assert
            Assert.True(deleteResult);
            Assert.Null(retrievedCollection);
            await mongoService.Delete(testCollection);
        }

        [Fact]
        public async Task UpdateService_ShouldModifyDocument()
        {
            // Arrange
            var serviceProvider = _service.BuildServiceProvider();
            var mongoService = serviceProvider.GetService<IMongoService<TestCollection>>();

            // Ensure mongoService is not null
            Assert.NotNull(mongoService);

            // Act
            var testCollection = new TestCollection
            {
                Email = "test@example.com",
                UserName = "testuser",
                Password = "Password123",
                ExpireTime = DateTime.UtcNow
            };

            await mongoService!.Create(testCollection);
            testCollection.UserName = "updateduser";
            var updateResult = await mongoService.Update(testCollection);
            var retrievedCollection = await mongoService.GetById(testCollection.Email);

            // Assert
            Assert.True(updateResult);
            Assert.NotNull(retrievedCollection);
            Assert.Equal("updateduser", retrievedCollection?.UserName);
            await mongoService.Delete(testCollection);

        }

        [Fact]
        public async Task GetAllService_ShouldReturnAllDocuments()
        {
            // Arrange

            var serviceProvider = _service.BuildServiceProvider();
            var mongoService = serviceProvider.GetService<IMongoService<TestCollection>>();

            // Ensure mongoService is not null
            Assert.NotNull(mongoService);

            // Act
            var testCollection1 = new TestCollection
            {
                Email = "test1@example.com",
                UserName = "testuser1",
                Password = "Password123",
                ExpireTime = DateTime.UtcNow
            };

            var testCollection2 = new TestCollection
            {
                Email = "test2@example.com",
                UserName = "testuser2",
                Password = "Password123",
                ExpireTime = DateTime.UtcNow
            };

            await mongoService!.Create(testCollection1);
            await mongoService.Create(testCollection2);
            var allCollections = await mongoService.GetAll();

            // Assert
            Assert.NotNull(allCollections);
            Assert.Equal(2, allCollections.Count());
            await mongoService.Delete(testCollection1);
            await mongoService.Delete(testCollection2);
            Assert.Equal(0,await mongoService.Count());
        }

        // A test collection class for testing purposes
        public class TestCollection
        {
            [BsonId][BsonElement("Email")] public string Email { get; set; } = string.Empty;

            [BsonElement("UserName")] public string UserName { get; set; } = string.Empty;

            [BsonElement("Password")] public string Password { get; set; } = string.Empty;

            [BsonElement("ExpireTime")]
            [BsonDateTimeOptions]
            public DateTime ExpireTime { get; set; }
        }
    }
}
