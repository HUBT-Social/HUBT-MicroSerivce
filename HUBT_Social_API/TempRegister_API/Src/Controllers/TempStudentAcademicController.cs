using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TempRegister_API.Src.Models;
using TempRegister_API.Src.Service;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HUBT_Social_Core.Models.Requests.Firebase;
using Microsoft.AspNetCore.Routing.Constraints;

namespace TempRegister_API.Src.Controllers

{
    public class DiemSinhVien
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string MaSV { get; set; } = string.Empty;
        public string TenMonHoc { get; set; } = string.Empty;
        public double Diem { get; set; }
    }

    public class SinhVien
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public long MASV { get; set; } 
        public string HoTen { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; } = string.Empty;
        public string TenLop { get; set; } = string.Empty;
    }


    [Route("api/temp-student-academic")]
    [ApiController]
    public class TempStudentAcademicController(
        IMongoService<TempCourse> tempCourse,
        IMongoService<TempCourseDetail> tempCourseDetail,
        IMongoService<TempStudentAcademic> tempStudentAcademic,
        IMongoService<DiemSinhVien> diem,
        IMongoService<SinhVien> sv,
        IOutService outService,
        INotifiService notifiService,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<TempCourse> _tempCourse = tempCourse;
        private readonly IMongoService<TempCourseDetail> _tempCourseDetail = tempCourseDetail;
        private readonly IOutService _outService = outService;
        private readonly INotifiService _notifiService = notifiService;
        private readonly IMongoService<TempStudentAcademic> _tempStudentAcademic = tempStudentAcademic;
        private readonly IMongoService<DiemSinhVien> _diem = diem;
        private readonly IMongoService<SinhVien> _sv = sv;

        //[HttpPost("create")]
        //public async Task<IActionResult> CreateStudentAcademic()
        //{
        //    try
        //    {

        //            var filter = Builders<SinhVien>.Filter.Empty;
        //            List<SinhVien>? sinhViens = await _sv.GetAll().ToListAsync();

        //        int current = 0;

        //            foreach (var s in sinhViens)
        //            {
        //            current++;

        //                var studentAcademic = new TempStudentAcademic
        //                {
        //                    Id = s.MASV.ToString(),
        //                    ClassName = s.TenLop,
        //                    Status = StudentStatus.Active,
        //                    SemesterResults = [],
        //                    Warnings = []
        //                };

        //                var result = await _tempStudentAcademic.Create(studentAcademic);
        //                Console.WriteLine(result ? $"✅ Created: {s.MASV}" : $"❌ Failed: {s.MASV}");
        //            }


        //        return Ok(new { message = "Đã tạo học lực sinh viên thành công." });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"❌ Exception: {ex.Message}");
        //        return StatusCode(500, new { message = "Lỗi khi tạo học lực sinh viên." });
        //    }
        //}
        private List<WarningHistory> GenerateWarnings(SemesterResult result)
        {
            var warnings = new List<WarningHistory>();
            var now = DateTime.Now;

            if (result.GPA < 1.0)
            {
                warnings.Add(new WarningHistory
                {
                    SemesterId = $"{result.Year}-{result.SemesterIndex}",
                    Type = WarningType.LowSemesterGPA,
                    Description = $"Điểm trung bình học kỳ thấp: {result.GPA:F2}",
                    CreatedAt = now
                });
            }

            if (result.ĐTBCHK < 1.4)
            {
                warnings.Add(new WarningHistory
                {
                    SemesterId = $"{result.Year}-{result.SemesterIndex}",
                    Type = WarningType.LowAccumulatedGPA,
                    Description = $"Điểm trung bình tích lũy thấp: {result.ĐTBCHK:F2}",
                    CreatedAt = now
                });
            }

            if (result.TotalFailedCreditsSoFar > 24)
            {
                warnings.Add(new WarningHistory
                {
                    SemesterId = $"{result.Year}-{result.SemesterIndex}",
                    Type = WarningType.TooManyFailedCredits,
                    Description = $"Tổng số tín chỉ bị F đã vượt quá 24.",
                    CreatedAt = now
                });
            }

            return warnings;
        }


        public static string ConvertScore10ToLetter(double score10)
        {
            if (score10 >= 8.5 && score10 <= 10) return "A";
            if (score10 >= 7.0 && score10 < 8.5) return "B";
            if (score10 >= 6.5 && score10 < 7.0) return "C+";
            if (score10 >= 5.5 && score10 < 6.5) return "C";
            if (score10 >= 4.0 && score10 < 5.5) return "D";
            return "F";
        }
        public static double ConvertScore10ToGPA4(double score10)
        {
            if (score10 >= 9.0 && score10 <= 10.0) return 4.0;
            if (score10 >= 8.5) return 3.7;
            if (score10 >= 8.0) return 3.5;
            if (score10 >= 7.0) return 3.0;
            if (score10 >= 6.5) return 2.5;
            if (score10 >= 5.5) return 2.0;
            if (score10 >= 5.0) return 1.5;
            if (score10 >= 4.0) return 1.0;
            return 0.0;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateStudentAcademic(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "Student ID is required" });
                }

                bool existingRecord = await _tempStudentAcademic.Exists(id);
                if (existingRecord)
                {
                    return Conflict(new { message = "Student academic record already exists" });
                }

                var studentAcademic = new TempStudentAcademic
                {
                    Id = id,
                    Status = StudentStatus.Active,
                    SemesterResults = [],
                    Warnings = []
                };

                await _tempStudentAcademic.Create(studentAcademic);

                var result = _mapper.Map<TempStudentAcademic>(studentAcademic);

                return Ok(new { message = "Student academic record created successfully", data = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while creating student academic record" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentAcademic(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "Student ID is required" });
                }

                var studentAcademic = await _tempStudentAcademic.GetById(id);
                if (studentAcademic == null)
                {
                    return NotFound(new { message = "Student academic record not found" });
                }

                var result = _mapper.Map<TempStudentAcademic>(studentAcademic);

                return Ok(new { message = "Student academic record retrieved successfully", data = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving student academic record" });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllStudentAcademics()
        {
            try
            {
                var studentAcademics = await _tempStudentAcademic.GetAll();
                var result = _mapper.Map<List<TempStudentAcademic>>(studentAcademics);

                return Ok(new { message = "Student academic records retrieved successfully", data = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving student academic records" });
            }
        }

        [HttpPut("update-replace-id={id}")]
        public async Task<IActionResult> UpdateStudentAcademic(string id, [FromBody] TempStudentAcademic updateModel)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "Student ID is required" });
                }

                if (updateModel == null)
                {
                    return BadRequest(new { message = "Update data is required" });
                }

                var existingRecord = await _tempStudentAcademic.GetById(id);
                if (existingRecord == null)
                {
                    return NotFound(new { message = "Student academic record not found" });
                }

                updateModel.Id = id; // Ensure ID is not changed
                var filter = Builders<TempStudentAcademic>.Filter.Eq(c => c.Id, id);
                var updated = await _tempStudentAcademic.Replace(filter, updateModel);

                if (updated)
                {
                    return StatusCode(500, new { message = "Failed to update student academic record" });
                }
                //
                var result = _mapper.Map<TempStudentAcademic>(updateModel);

                return Ok(new { message = "Student academic record updated successfully", data = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating student academic record" });
            }
        }

        [HttpPut("update-new-semester")]
        public async Task<IActionResult> UpdateSemester(string id, [FromBody] SemesterRequest result)
        {
            // Kiểm tra điều kiện đầu vào
            var validationResult = ValidateValueUpdateSemester(id, result);
            if (validationResult != null)
            {
                return validationResult; // Trả về ngay nếu có lỗi
            }
            var studentAcademic = await _tempStudentAcademic.GetById(id);
            if (studentAcademic == null) { return BadRequest("Khong tim thay nguoi dung voi id tuong ung!"); }

            var lastSemsterResult = studentAcademic.SemesterResults.LastOrDefault();
            SemesterResult semesterResult = new()
            {
                Year = result.Year,
                SemesterIndex = result.SemesterIndex,
                Subjects = result.Subjects,
            };
            GenarateSemesterResult(studentAcademic, semesterResult);

            studentAcademic.SemesterResults.Add(semesterResult);

            _ = Task.Run(() =>
            {
                var notificationcontent = CheckSemesterResult(id, studentAcademic, semesterResult);
                if (notificationcontent != null) 
                {
                    _ = _notifiService.SendRemindNotication(notificationcontent).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine("Gửi thông báo nhắc nhở thất bại: ", task.Exception);
                        }
                    });
                }

            });
            var filter = Builders<TempStudentAcademic>.Filter.Eq(c => c.Id, id);
            bool updateSuccesed = await _tempStudentAcademic.Replace(filter,studentAcademic);

            return updateSuccesed ? Ok("Updated semester successful.") : BadRequest("Update faild.");
        }

        private static void GenarateSemesterResult(TempStudentAcademic studentAcademic, SemesterResult newResult)
        {
            SemesterResult? lastResult = studentAcademic.SemesterResults.LastOrDefault();

            int subjectCount = newResult.Subjects.Count;
            double totalScore = 0.0;
            int total_F_Score = 0;

            if (lastResult == null)
            {
                newResult.Subjects.ForEach(s => {
                    double score4 = ConvertScore10ToGPA4(s.GradePoint);
                    totalScore += score4;
                    string scoreLetter = ConvertScore10ToLetter(s.GradePoint);
                    s.LetterGrade = scoreLetter;
                    if (scoreLetter == "F")
                    {
                        total_F_Score += 1;
                    }
                });
                double DTBHKorGPA = totalScore/ newResult.Subjects.Count();
                newResult.GPA = DTBHKorGPA;
                newResult.ĐTBCHK = DTBHKorGPA;
                newResult.TotalFailedCreditsSoFar = total_F_Score;
                return;
            };

            total_F_Score = lastResult.TotalFailedCreditsSoFar;
            newResult.Subjects.ForEach(s => {
                double score4 = ConvertScore10ToGPA4(s.GradePoint);
                totalScore += score4;
                string scoreLetter = ConvertScore10ToLetter(s.GradePoint);
                s.LetterGrade = scoreLetter;
                if (scoreLetter == "F")
                {
                    total_F_Score += 1;
                }
            });
            double DTBHK = totalScore / newResult.Subjects.Count();
            int semmesterCount = studentAcademic.SemesterResults.Count();
            newResult.GPA = (DTBHK+ lastResult.GPA* semmesterCount) / (semmesterCount+1);
            newResult.ĐTBCHK = DTBHK;
            newResult.TotalFailedCreditsSoFar = total_F_Score;
            return;
        }
        private static List<NotificatonRemindRequest>? CheckSemesterResult(string id, TempStudentAcademic studentAcademic, SemesterResult newResult)
        {
            var result = new List<NotificatonRemindRequest>();
            SemesterResult? lastResult = studentAcademic.SemesterResults.LastOrDefault();
            // Kiểm tra id hợp lệ
            if (string.IsNullOrEmpty(id))
            {
                
                return null;
            }

            // Kiểm tra newResult không null
            if (newResult == null)
            {
              
                return null;
            }

            bool isDtbtlLow = false;
            bool dtbhkLow = false;
            bool isFailedCreditsHigh = false;
            double dtbtlThreshold = 0;

            // Hàm hỗ trợ kiểm tra điều kiện cảnh báo học tập
            bool IsStudyWarning(SemesterResult semester)
            {
                // Điều kiện ĐTBCHK theo năm học
                dtbtlThreshold = semester.Year switch
                {
                    1 => 1.2,
                    2 => 1.4,
                    3 => 1.6,
                    _ => 1.8 // Năm 4 và các năm tiếp theo
                };
                isDtbtlLow = semester.GPA < dtbtlThreshold;

                // Điều kiện GPA theo học kỳ
                dtbhkLow = (semester.Year == 1 && semester.SemesterIndex == 1)
                    ? semester.ĐTBCHK < 0.8
                    : semester.ĐTBCHK < 1.0;

                // Điều kiện tín chỉ F
                isFailedCreditsHigh = semester.TotalFailedCreditsSoFar > 24;

                return isDtbtlLow || dtbhkLow || isFailedCreditsHigh;
                
            }

            bool isWarning = IsStudyWarning(newResult);

            // Hàm hỗ trợ tạo nội dung kết quả học kỳ
            string FormatSemesterResult(SemesterResult semester)
            {
                var subjects = string.Join("; ", semester.Subjects.Select(s => $"{s.SubjectName}: {s.LetterGrade} ({s.GradePoint})"));
                return $"Kết quả học kỳ {semester.Year}-{semester.SemesterIndex}: GPA = {semester.GPA}, ĐTBCHK = {semester.ĐTBCHK}, Tổng tín chỉ F = {semester.TotalFailedCreditsSoFar}";
            }

            string extraContent = studentAcademic.Status == StudentStatus.Warning
                        ? "Tuy nhiên đây là lần thứ hai liên tiếp bạn bị cảnh báo học tập, đo đó rất tiêc khi phải thông báo rằng bạn đã bị buộc thôi học, vui lòng liên hệ phía nhà trường để giải quyết. Xin cảm ơn."
                        : "Vui lòng chú ý học tập hơn cho kì học tiếp theo, nếu kì sau bạn còn bị cảnh báo thì sẽ phải buộc thôi học. Xin cảm ơn";

            if (lastResult == null)
            {
                // Trường hợp chỉ có newResult
                if (isWarning)
                {
                    // Cảnh báo học tập với giọng điệu nghiêm túc, khẩn cấp
                    var warningDetails = new List<string>();
                    if (isDtbtlLow)
                    {
                        warningDetails.Add($"Điểm trung bình tích lũy của bạn ({newResult.GPA}) đang thấp hơn ngưỡng {dtbtlThreshold} yêu cầu cho năm học {newResult.Year}.");
                    }
                    if (dtbhkLow)
                    {
                        warningDetails.Add($"Điểm trung bình học kỳ ({newResult.GPA}) không đạt mức {(newResult.Year == 1 && newResult.SemesterIndex == 1 ? 0.8 : 1.0)} theo quy định.");
                    }
                    if (newResult.TotalFailedCreditsSoFar > 24)
                    {
                        warningDetails.Add($"Bạn đã tích lũy {newResult.TotalFailedCreditsSoFar} tín chỉ F, vượt quá giới hạn 24 tín chỉ cho phép");
                    }


                    studentAcademic.Status = StudentStatus.Warning;

                    result.Add(
                        new NotificatonRemindRequest {
                            UserName = id,
                            RemindCode = "learning_alerts",
                            Content = $"Cảnh báo học tập: Bạn vừa hoàn thành kì học đầu tiên rất tiêc khi phải nhắc nhở bạn rằng: {string.Join(" ", warningDetails)} {extraContent}"
                        });
                }
                else if (newResult.GPA >= 3.0)
                {
                    // Chúc mừng điểm cao với giọng điệu vui vẻ, khích lệ
                    result.Add(new NotificatonRemindRequest
                    {
                        UserName = id,
                        RemindCode = "normal",
                        Content = $"Kết quả học tập: Chúc mừng bạn đã xuất sắc đạt GPA cao cho học kì đầu tiên! {FormatSemesterResult(newResult)} Đây là thành tích tuyệt vời, hãy tiếp tục phát huy nhé!"
                    });
                    studentAcademic.Status = StudentStatus.Active;

                }
                else
                {
                    // Thông báo bình thường với giọng điệu trung tính
                    result.Add(new NotificatonRemindRequest
                    {
                        UserName = id,
                        RemindCode = "normal",
                        Content = $"Thông báo kết quả học kỳ đầu tiên: {FormatSemesterResult(newResult)} Vui lòng kiểm tra chi tiết và tiếp tục nỗ lực trong kỳ tới."
                    });
                    studentAcademic.Status = StudentStatus.Active;
                }
            }
            else
            {
                // Trường hợp có cả lastResult và newResult
                bool isProgress = newResult.GPA > lastResult.GPA || newResult.ĐTBCHK > lastResult.ĐTBCHK;
                bool isRegress = newResult.GPA < lastResult.GPA || newResult.ĐTBCHK < lastResult.ĐTBCHK;

                if (isWarning)
                {
                    // Cảnh báo học tập với giọng điệu nghiêm túc, khẩn cấp
                    var warningDetails = new List<string>();
                    if (isDtbtlLow)
                    {
                        warningDetails.Add($"Điểm trung bình tích lũy của bạn ({newResult.GPA}) đang thấp hơn ngưỡng {dtbtlThreshold} yêu cầu cho năm học {newResult.Year}.");
                    }
                    if (dtbhkLow)
                    {
                        warningDetails.Add($"Điểm trung bình học kỳ ({newResult.GPA}) không đạt mức {(newResult.Year == 1 && newResult.SemesterIndex == 1 ? 0.8 : 1.0)} theo quy định.");
                    }
                    if (newResult.TotalFailedCreditsSoFar > 24)
                    {
                        warningDetails.Add($"Bạn đã tích lũy {newResult.TotalFailedCreditsSoFar} tín chỉ F, vượt quá giới hạn 24 tín chỉ cho phép");
                    }


                    studentAcademic.Status = StudentStatus.Warning;

                    result.Add(
                        new NotificatonRemindRequest
                        {
                            UserName = id,
                            RemindCode = "learning_alerts",
                            Content = $"Cảnh báo học tập: Bạn vừa hoàn thành học kì {newResult.SemesterIndex} của năm {newResult.Year} rất tiêc khi phải nhắc nhở bạn rằng: {string.Join(" ", warningDetails)} {extraContent}"
                        });
                }
                else if (isProgress)
                {
                    // Chúc mừng tiến bộ với giọng điệu vui vẻ, khích lệ
                    result.Add(new NotificatonRemindRequest
                    {
                        UserName = id,
                        RemindCode = "normal",
                        Content = $"Thật tuyệt vời! Bạn đã có tiến bộ vượt bậc trong kỳ {newResult.SemesterIndex} của năm {newResult.Year}! GPA tăng từ {lastResult.GPA} lên {newResult.GPA}, ĐTBCHK tăng từ {lastResult.ĐTBCHK} lên {newResult.ĐTBCHK}. Hãy tiếp tục giữ vững phong độ này nhé!"
                    });
                    studentAcademic.Status = StudentStatus.Active;
                }
                else if (isRegress)
                {
                    // Nhắc nhở suy giảm với giọng điệu quan tâm, khuyến khích
                    result.Add(new NotificatonRemindRequest
                    {
                        UserName = id,
                        RemindCode = "normal",
                        Content = $"Kết quả kỳ {newResult.SemesterIndex} của năm {newResult.Year} của bạn có phần giảm sút so với kỳ trước (GPA: {lastResult.GPA} -> {newResult.GPA}, ĐTBCHK: {lastResult.ĐTBCHK} -> {newResult.ĐTBCHK}). Hãy cố gắng hơn trong kỳ tới để lấy lại phong độ nhé! (type = StudyAlert)."
                    });
                    studentAcademic.Status = StudentStatus.Active;
                }
                else
                {
                    // Thông báo bình thường với giọng điệu trung tính
                    result.Add(new NotificatonRemindRequest
                    {
                        UserName = id,
                        RemindCode = "normal",
                        Content = $"Thông báo kết quả học kỳ {newResult.SemesterIndex} của năm {newResult.Year}: {FormatSemesterResult(newResult)} Vui lòng kiểm tra chi tiết và tiếp tục nỗ lực trong kỳ tới."
                    });
                    studentAcademic.Status = StudentStatus.Active;
                }
            }

            return result;
        }

        private IActionResult? ValidateValueUpdateSemester(string id, SemesterRequest result)
        {
            // Kiểm tra id không được null hoặc rỗng
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { error = "ID học kỳ không được để trống." });
            }

            // Kiểm tra result không được null
            if (result == null)
            {
                return BadRequest(new { error = "Dữ liệu kết quả học kỳ không được null." });
            }

            // Kiểm tra year hợp lệ (từ 1 đến 6)
            if (result.Year < 1 || result.Year > 6)
            {
                return BadRequest(new { error = "Năm học phải nằm trong khoảng từ 1 đến 6." });
            }

            // Kiểm tra semesterindex hợp lệ (1 hoặc 2)
            if (result.SemesterIndex >= 1)
            {
                return BadRequest(new { error = "Học kỳ phải là 1 trở nên" });
            }

            // Kiểm tra subjects không được null hoặc rỗng
            if (result.Subjects == null || !result.Subjects.Any())
            {
                return BadRequest(new { error = "Danh sách môn học không được null hoặc rỗng." });
            }
            return null;
        }

        // Hàm hỗ trợ kiểm tra tính nhất quán giữa GradePoint và LetterGrade
        private bool IsValidGradePointForLetterGrade(double gradePoint, string letterGrade)
        {
            return letterGrade switch
            {
                "A" => gradePoint >= 3.7 && gradePoint <= 4.0,
                "B+" => gradePoint >= 3.3 && gradePoint < 3.7,
                "B" => gradePoint >= 3.0 && gradePoint < 3.3,
                "C+" => gradePoint >= 2.3 && gradePoint < 3.0,
                "C" => gradePoint >= 2.0 && gradePoint < 2.3,
                "D" => gradePoint >= 1.0 && gradePoint < 2.0,
                "F" => gradePoint == 0.0,
                _ => false
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentAcademic(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "Student ID is required" });
                }

                var Record = await _tempStudentAcademic.GetById(id);
                if (Record == null)
                {
                    return NotFound(new { message = "Student academic record not found" });
                }

                var deleted = await _tempStudentAcademic.Delete(Record);
                if (deleted)
                {
                    return StatusCode(500, new { message = "Failed to delete student academic record" });
                }

                return Ok(new { message = "Student academic record deleted successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting student academic record" });
            }
        }
    }
}