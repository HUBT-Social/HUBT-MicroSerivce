using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.ExamDTO;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using TempRegister_API.Src.Models;

namespace TempRegister_API.Src.Controllers
{
    [Route("api/tempexam")]
    [ApiController]
    public class TempExamController(
        IMongoService<TempExam> tempExam,
        IMongoService<TempQuestion> tempQuestion,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<TempExam> _tempExam = tempExam;
        private readonly IMongoService<TempQuestion> _tempQuestion = tempQuestion;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                TempExam? exam = await _tempExam.GetById(id);
                if (exam != null)
                {
                    ExamDTO examDTO = _mapper.Map<ExamDTO>(exam);
                    return Ok(examDTO);
                }
                return NotFound(new { message = "Exam not found" });
            }
            return BadRequest("Either id or className must be provided");
        }
        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestions([FromQuery] string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                List<TempQuestion> questions = await _tempQuestion.Find(q => q.ExamId == id).ToListAsync();
                if (questions.Count != 0)
                {
                    List<Question> questionDTOs = _mapper.Map<List<Question>>(questions);
                    return Ok(questionDTOs.ToArray());
                }
                return NotFound(new { message = "Exam not found" });
            }
            return BadRequest("Either id or className must be provided");
        }
        [HttpGet("major")]
        public async Task<IActionResult> GetList([FromQuery] string? major, [FromQuery] int page = 0, [FromQuery] int limit = 10)
        {
            // Nếu major null hoặc rỗng, không dùng filter (tức là lấy tất cả)
            Expression<Func<TempExam, bool>>? predicate = null;

            if (!string.IsNullOrWhiteSpace(major))
            {
                predicate = e => e.Major.Equals(major, StringComparison.OrdinalIgnoreCase);
            }

            // Nếu page/limit = 0 thì không phân trang
            int? pageArg = page > 0 ? page : null;
            int? limitArg = limit > 0 ? limit : null;

            var exams = await _tempExam.Find(predicate, pageArg, limitArg);

            if (exams.Any())
            {
                var examDTOs = _mapper.Map<List<ExamDTO>>(exams);
                return Ok(examDTOs);
            }

            return NotFound(new { message = "Exams not found" });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] QuizDetail request)
        {
            if (request == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            request.Id = string.Empty;
            try
            {
                
                TempExam exam = _mapper.Map<TempExam>(request);
                exam.QuestionCount = request.QuestionCount;
                bool isCreated = await _tempExam.Create(exam);
                List<TempQuestion> tempQuestions = [];
                foreach (Question question in request.Questions)
                {
                    TempQuestion tempQuestion = new()
                    {
                        ExamId = exam.Id,
                        Title = question.Title,
                        Answers = question.Answers,
                        CorrectAnswer = question.CorrectAnswer
                    };
                    tempQuestions.Add(tempQuestion);
                    // Ensure each question has a new ID
                }
                bool isQuestionCreated = await _tempQuestion.CreateMany(tempQuestions);
                if (!isQuestionCreated)
                {
                    Console.WriteLine($"Failed to create question in database. {tempQuestions}");
                }
                ExamDTO examDTO = _mapper.Map<ExamDTO>(exam);
                return isCreated ? 
                    Ok(examDTO) : 
                    BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
            }
        }
        [HttpPost("questions")]
        public async Task<IActionResult> QuestionPost([FromBody] TempQuestion question)
        {
            if (question == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            try
            {
                Question questionDTO = _mapper.Map<Question>(question);

                return await _tempQuestion.Create(question) ?
                    Ok(questionDTO) :
                    BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
            }
        }
    }
}
