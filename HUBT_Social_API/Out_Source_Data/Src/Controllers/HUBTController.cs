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
        IMongoService<HocPhan> course,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<SinhVien> _student = student;
        private readonly IMongoService<Diemtb> _aGVScore = aGVScore;
        private readonly IMongoService<ThoiKhoaBieu> _timeTable = timeTable;
        private readonly IMongoService<DiemSinhVien> _score = score;
        private readonly IMongoService<HocPhan> _course = course;

        [HttpGet("sinhvien")]
        public async Task<IActionResult> GetStudentData([FromQuery] string masv)
        {
            if (string.IsNullOrEmpty(masv)) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            SinhVien? studentInfo = await _student.Find(p => p.Masv == masv).FirstOrDefaultAsync();
            if (studentInfo == null) return NotFound(LocalValue.Get(KeyStore.UserNotFound));
            StudentDTO studentDTO = _mapper.Map<StudentDTO>(studentInfo);
            return Ok(studentDTO);
        }
        [HttpGet("sinhvien/{className}")]
        public async Task<IActionResult> GetStudentList([FromRoute] string className)
        {
            List<SinhVien> students = await _student.Find(p=> p.TenLop == className).ToListAsync();
            if (students == null || students.Count == 0) return NotFound(LocalValue.Get(KeyStore.UserNotFound));
            List<StudentDTO> studentDTOs = _mapper.Map<List<StudentDTO>>(students);
            return Ok(studentDTOs);
        }
        [HttpGet("sinhvien/{masv}/diem")]
        public async Task<IActionResult> GetStudentScore2([FromRoute] string masv)
        {
            if (string.IsNullOrEmpty(masv)) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            List<DiemSinhVien> score = await _score.Find(p => p.Masv == masv).ToListAsync();
            if (score != null) {
                List<ScoreDTO> scoreDTO = _mapper.Map<List<ScoreDTO>>(score);
                return Ok(scoreDTO);
            }
            return NotFound(LocalValue.Get(KeyStore.UserNotFound));
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
        public async Task<IActionResult> GetStudentTimeTable([FromQuery] string? className, [FromQuery] string? thu, [FromQuery] string? id)
        {
            if (string.IsNullOrEmpty(className) && string.IsNullOrEmpty(id)) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));

            List<ThoiKhoaBieu> timeTable = [];

            if (!string.IsNullOrEmpty(id))
            {
                // Search by id
                var timeTableEntry = await _timeTable.GetById(id);
                if (timeTableEntry != null)
                {
                    timeTable.Add(timeTableEntry);
                }
            }

            if (!string.IsNullOrEmpty(className))
            {
                // Search by className and optionally by thu
                var classTimeTable = await _timeTable.Find(p => p.ClassName == className).ToListAsync();
                if (classTimeTable != null && classTimeTable.Count > 0)
                {
                    if (!string.IsNullOrEmpty(thu))
                    {
                        classTimeTable = classTimeTable.Where(p => p.Day == thu).ToList();
                    }
                    timeTable.AddRange(classTimeTable);
                }
            }

            if (timeTable.Count > 0)
            {
                List<TimeTableDTO> timeTableDTOs = _mapper.Map<List<TimeTableDTO>>(timeTable);
                return Ok(timeTableDTOs);
            }

            return NotFound(LocalValue.Get(KeyStore.UserNotFound));
        }
        [HttpGet("hocphan")]
        public async Task<IActionResult> GetCourses([FromQuery] string major, [FromQuery] int? course = null)
        {

            List<HocPhan> hocPhans = await _course.Find(hp => hp.Manganh.Equals(major, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
            if (hocPhans.Count <= 0)
                return BadRequest();
            try
            {
                if (course != null)
                {
                    hocPhans = hocPhans.Where(hp => hp.Khoa >=  course).ToList();
                }
                List<CouresDTO> coures = _mapper.Map<List<CouresDTO>>(hocPhans);
                return Ok(hocPhans);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            return BadRequest();
        }

        //[HttpPost("create")]
        //public async Task<IActionResult> CreateStudentData()
        //{
        //    HocPhan diem = new()
        //    {
        //        Khoa = 27,
        //        Tenmon = "Lap trinh C#",
        //        Manganh = "TH",
        //        Sotinchi = 3
        //    };
        //    if (diem == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        //    await _course.Create(diem);
        //    return Ok(LocalValue.Get(KeyStore.FileUploadedSuccessfully));
        //}

    }
}
