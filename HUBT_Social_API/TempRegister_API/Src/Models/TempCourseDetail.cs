    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;

    namespace TempRegister_API.Src.Models

{ 
    /// /////////////////////////////////////////////// CAC MODEL PHUC VU NHAC NHO THEO MON ///////////////////////////////////////////////////////

    public class TempCourseDetail
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CourseName { get; set; }
        public string ClassName { get; set; }
        public int Credits  { get; set; }
        public string[] Instructor { get; set; }
        public int CurrentWeek { get; set; } = 0;

        // Thay đổi từ List<Student> thành Dictionary<string, Student>
        // Key: StudentId, Value: Student object
        public Dictionary<string, Student> Students { get; set; } = new Dictionary<string, Student>();
    }

    public class Student
    {
        public string StudentId { get; set; }
        public string? Name { get; set; }

        // Thay đổi từ List<Attendance> thành Dictionary<int, AttendanceStatus>
        // Key: Week, Value: AttendanceStatus
        public Dictionary<string, AttendanceStatus> AttendanceRecords { get; set; } = new Dictionary<string, AttendanceStatus>();

        // Thay đổi từ List<Score> thành Dictionary<string, double>
        // Key: Score Name, Value: Score Value
        public Dictionary<string, double> ClassScores { get; set; } = [];

        public double? EntrepreneurshipScore { get; set; }
        public double? ModuleScore { get; set; }
        public double? ExamScore { get; set; }
        public double? TotalScore { get; set; }
        public bool IsKDT { get; set; } = false;
    }

    public enum AttendanceStatus
    {
        Present = 0,
        Late = 1,  
        Absent = 2,    
    }

    // ---------------------- Request DTOs ----------------------
    public class AttendanceRecord
    {
        public string StudentId { get; set; }
        public AttendanceStatus Status { get; set; }
    }

    public class AttendanceRequest
    {
        public int Week { get; set; } // <-- Sửa ở đây
        public List<AttendanceRecord> Records { get; set; }
    }

    public class ScoreRequest
    {
        public string StudentId { get; set; }
        public string ScoreName { get; set; }
        public double ScoreValue { get; set; }
    }



    /// /////////////////////////////////////////////// CAC MODEL PHUC VU CANH BAO ///////////////////////////////////////////////////////
    public enum StudentStatus
    {
        Active,         // Đang học bình thường
        Warning,        // Bị cảnh báo
        Expelled,       // Buộc thôi học
        Graduated       // Tốt nghiệp
    }
    public enum WarningType
    {
        LowSemesterGPA,
        LowAccumulatedGPA,
        TooManyFailedCredits
    }
    public class WarningHistory
    {
        public string SemesterId { get; set; }
        public WarningType Type { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class SemesterResult
    {
        public int Year { get; set; }                  // Năm học
        public int SemesterIndex { get; set; }         // 1 hoặc 2
        public double GPA { get; set; }                // Điểm trung bình học kỳ (ĐTBCHK)
        public List<SubjectResult> Subjects { get; set; } = [];
        public double ĐTBCHK { get; set; }     // ĐTBCTL tính đến học kỳ này
        public int TotalFailedCreditsSoFar { get; set; } // Tổng tín chỉ bị F tính đến hiện tại
    }
    public class SubjectResult
    {
        public string SubjectName { get; set; }
        public double GradePoint { get; set; }        // Thang điểm 4
        public string LetterGrade { get; set; }       // A, B, C, D, F
    }

    public class TempStudentAcademic
    {
        public string Id { get; set; }                 // Mã 
        public string ClassName { get; set; }
        public StudentStatus Status { get; set; }      // Active, Warning, Expelled, Graduated
        public List<SemesterResult> SemesterResults { get; set; } = new();
        public List<WarningHistory> Warnings { get; set; } = new();
    }


}
