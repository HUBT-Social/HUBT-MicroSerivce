using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Firebase.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestLibary.Service;


namespace TestLibary
{
    public class TrivalDataTest
    {
        private readonly IConfiguration _configuration;
        private readonly ServiceCollection _service;
        private readonly ITestService _testService;

        public TrivalDataTest()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _service = new ServiceCollection();
            _service.AddHttpClientService();
            string? identityPath = _configuration.GetSection("HUBT_Data").Get<string>() ?? throw new Exception("Unfound Section");
            _service.AddRegisterClientService<ITestService, TestService>(identityPath);
            _testService = _service.BuildServiceProvider().GetService<ITestService>() ?? throw new Exception("Unfound Section");
        }
        [Fact]
        public async void Trival_HUBT_data()
        {
            // Arrange
            StudentDTO? studentDTO = await _testService.GetStudentByMasv("2722245080");
            List<ScoreDTO>? studentScoreDTO = await _testService.GetStudentScoreByMasv("2722245080");
            AVGScoreDTO? avgScoreDTODTO = await _testService.GetAVGScoreByMasv("2722245080");
            List<TimeTableDTO>? timeTableDTO = await _testService.GetTimeTableByClassName("Th27.25");

            //Assert
            Assert.NotNull(studentDTO);
            Assert.NotNull(studentScoreDTO);
            Assert.NotNull(avgScoreDTODTO);
            Assert.NotNull(timeTableDTO);

        }
    }
}
