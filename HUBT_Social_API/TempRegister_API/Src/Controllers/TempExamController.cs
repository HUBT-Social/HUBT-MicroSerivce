using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.ExamDTO;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TempRegister_API.Src.Models;

namespace TempRegister_API.Src.Controllers
{
    [Route("api/tempexam")]
    [ApiController]
    public class TempExamController(
        IMongoService<TempExam> tempExam,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<TempExam> _tempExam = tempExam;

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
        [HttpGet("major")]
        public async Task<IActionResult> GetList([FromQuery] string? major)
        {
            if (!string.IsNullOrEmpty(major))
            {
                List<TempExam> exams = await _tempExam.Find(e => e.Major.Equals(major,StringComparison.OrdinalIgnoreCase)).ToListAsync();
                if (exams != null)
                {
                    List<ExamDTO> examDTOs = _mapper.Map<List<ExamDTO>>(exams);
                    return Ok(examDTOs);
                }
                return NotFound(new { message = "Exams not found" });
            }
            return BadRequest("Either id or className must be provided");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ExamDTO examDTO)
        {
            if (examDTO == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            examDTO.Id = string.Empty;
            try
            {
                TempExam exam = _mapper.Map<TempExam>(examDTO);

                return await _tempExam.Create(exam) ? 
                    Ok(LocalValue.Get(KeyStore.FileUploadedSuccessfully)): 
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
