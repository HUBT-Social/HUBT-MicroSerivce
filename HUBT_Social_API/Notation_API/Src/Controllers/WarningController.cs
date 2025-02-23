using HUBT_Social_Core.Models.OutSourceDataDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notation_API.Src.Services;

namespace Notation_API.Src.Controllers
{
    [Route("api/notation")]
    [ApiController]
    public class WarningController(INotationService notationService) : ControllerBase
    {
        private readonly INotationService _notationService = notationService;

        [HttpGet("get-avg-score")]
        public async Task<IActionResult> GetAVGScoreByMasv(string masv)
        {
            AVGScoreDTO? result = await _notationService.GetAVGScoreByMasv(masv);
            return result != null ? Ok(result) : NotFound();
        }
        [HttpGet("get-student")]
        public async Task<IActionResult> GetStudentByMasv(string masv)
        {
            StudentDTO? result = await _notationService.GetStudentByMasv(masv);
            return result != null ? Ok(result) : NotFound();
        }
        [HttpGet("get-student-score")]
        public async Task<IActionResult> GetStudentScoreByMasv(string masv)
        {
            List<ScoreDTO>? result = await _notationService.GetStudentScoreByMasv(masv);
            return result != null ? Ok(result) : NotFound();
        }
        [HttpGet("get-time-table")]
        public async Task<IActionResult> GetTimeTableByClassName(string className)
        {
            List<TimeTableDTO>? result = await _notationService.GetTimeTableByClassName(className);
            return result != null ? Ok(result) : NotFound();
        }
    }
}
