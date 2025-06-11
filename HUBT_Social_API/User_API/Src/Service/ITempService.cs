using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.ExamDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings.@enum;

namespace User_API.Src.Service
{
    public interface ITempService : IBaseService
    {
        Task<TimetableOutputDTO> StoreInTimeTable(TimetableOutputDTO request);
        Task<List<TimetableOutputDTO>> StoreInTimeTable(List<TimetableOutputDTO> request);


        Task<TimetableOutputDTO> UpdateTimetable(UpdateTimetableRequest request);
        Task<List<TimetableOutputDTO>> GetListOfTimeTableByClassName(string className);
        Task<List<TimetableOutputDTO>> GetTimetable(string id = "",string className = "", string coursesId= "");

        Task<ClassScheduleVersionDTO> GetClassScheduleVersion(string className);
        Task<List<CouresDTO>> GetCourses(string userName = "", string className = "", string id = "");

        Task<ClassScheduleVersionDTO> StoreClassScheduleVersion(string className,DateTime expireTime);
        Task<ClassScheduleVersionDTO> StoreClassScheduleVersion(ClassScheduleVersionDTO request);
        Task<CouresDTO> StoreCourses(CreateTempCourseRequest request);

        Task<ExamDTO> StoreExam(QuizDetail request);

        Task<List<ExamDTO>> GetExams(string? major,int page = 0,int limit = 10);

        Task<ExamDTO?> GetExam(string id);
        Task<Question[]> GetExamQuestions(string id);


    }
}
