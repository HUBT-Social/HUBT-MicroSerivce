using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Out_Source_Data.Src.Models;

namespace Out_Source_Data.Src.Controllers
{
    [Route("api/hubt")]
    [ApiController]
    public class HUBTController(
        IMongoService<SinhVien> student,
        IMongoService<Diemtb> aGVScore,
        IMongoService<ThoiKhoaBieu> timeTable,
        IMongoService<DiemSinhVien> score,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<SinhVien> _student = student;
        private readonly IMongoService<Diemtb> _aGVScore = aGVScore;
        private readonly IMongoService<ThoiKhoaBieu> _timeTable = timeTable;
        private readonly IMongoService<DiemSinhVien> _score = score;
        [HttpGet("sinhvien")]
        public async Task<IActionResult> GetStudentData([FromQuery] string masv)
        {
            if (string.IsNullOrEmpty(masv)) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            SinhVien? studentInfo = await _student.Find(p => p.Masv == masv).FirstOrDefaultAsync();
            if (studentInfo == null) return NotFound(LocalValue.Get(KeyStore.UserNotFound));
            StudentDTO studentDTO = _mapper.Map<StudentDTO>(studentInfo);
            return Ok(studentDTO);
        }
        [HttpGet("sinhvien/diem")]
        public async Task<IActionResult> GetStudentScore([FromQuery] string masv)
        {
            if (string.IsNullOrEmpty(masv)) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            List<DiemSinhVien> score = await _score.Find(p => p.Masv == masv).ToListAsync();
            if (score == null) return NotFound(LocalValue.Get(KeyStore.UserNotFound));
            List<ScoreDTO> scoreDTO = _mapper.Map<List<ScoreDTO>>(score);
            return BaseOk(scoreDTO);
        }
        [HttpGet("diemtb")]
        public async Task<IActionResult> GetStudentAvgScore([FromQuery] string masv)
        {
            if (string.IsNullOrEmpty(masv)) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            Diemtb? aGVScore = await _aGVScore.Find(p => p.MaSV == masv).FirstOrDefaultAsync();
            if (aGVScore == null) return NotFound(LocalValue.Get(KeyStore.UserNotFound));
            AVGScoreDTO aVGScoreDTO = _mapper.Map<AVGScoreDTO>(aGVScore);
            return Ok(aVGScoreDTO);
        }
        [HttpGet("thoikhoabieu")]
        public async Task<IActionResult> GetStudentTimeTable([FromQuery] string className)
        {
            if (string.IsNullOrEmpty(className)) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            List<ThoiKhoaBieu> timeTable = await _timeTable.Find(p => p.ClassName == className).ToListAsync();
            if (timeTable == null) return NotFound(LocalValue.Get(KeyStore.UserNotFound));
            
            List<TimeTableDTO> timeTableDTOs = _mapper.Map<List<TimeTableDTO>>(timeTable);
            return Ok(timeTableDTOs);
        }
        //[HttpPost("create")]
        //public async Task<IActionResult> CreateStudentData()
        //{
        //    DiemSinhVien diem = new()
        //    {
        //        Masv = "B20DCCN001",
        //        TenMonHoc = "Lap trinh C#",
        //        Diem = 10
        //    };
        //    if (diem == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        //    await _score.Create(diem);
        //    return Ok(LocalValue.Get(KeyStore.FileUploadedSuccessfully));
        //}

    }
}
