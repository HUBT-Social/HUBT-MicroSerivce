using Amazon.Runtime.Internal;
using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.ExamDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.Settings.@enum;
using System.Net;

namespace User_API.Src.Service
{
    public class TempService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), ITempService
    {
        public async Task<TimetableOutputDTO> GetTimetable(string id)
        {
            string path = APIEndPoint.TempUrls.TempTimetableTimetable
                .BuildUrl(
                    new Dictionary<string, string> { { "id", id } }
                );
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.GET);
            return responseDTO.ConvertTo<TimetableOutputDTO>() ?? new();
        }
        public async Task<List<TimetableOutputDTO>> GetListOfTimeTableByClassName(string className)
        {
            string path = APIEndPoint.TempUrls.TempTimetableTimetable
                .BuildUrl(
                    new Dictionary<string, string> { { "className", className } }
                );
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.GET);
            return responseDTO.ConvertTo<List<TimetableOutputDTO>>() ?? [];
        }

        public async Task<TimetableOutputDTO> StoreInTimeTable(TimetableOutputDTO request)
        {
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempTimetableTimetable, ApiType.POST, request);
            return responseDTO.ConvertTo<TimetableOutputDTO>() ?? new();
        }
        public async Task<List<TimetableOutputDTO>> StoreInTimeTable(List<TimetableOutputDTO >request)
        {
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempTimetableCreateTimetable, ApiType.POST, request);
            return responseDTO.ConvertTo<List<TimetableOutputDTO>>() ?? [];
        }
        public async Task<ClassScheduleVersionDTO> GetClassScheduleVersion(string className)
        {
            string path = APIEndPoint.TempUrls.TempTimetableGetClassScheduleVersion
                .BuildUrl(
                    new Dictionary<string, string> { { "className", className } }
                );
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.GET);
            return responseDTO.ConvertTo<ClassScheduleVersionDTO>() ?? new();
        }

        public async Task<ClassScheduleVersionDTO> StoreClassScheduleVersion(string className, DateTime expireTime)
        {
            ClassScheduleVersionDTO request = new()
            {
                ClassName = className,
                ExpireTime = expireTime
            };
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempTimetableCreateClassScheduleVersion, ApiType.POST, request);
            return responseDTO.ConvertTo<ClassScheduleVersionDTO>() ?? new();
        }
        public async Task<ClassScheduleVersionDTO> StoreClassScheduleVersion(ClassScheduleVersionDTO request)
        {   
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempTimetableCreateClassScheduleVersion, ApiType.POST, request);
            return responseDTO.ConvertTo<ClassScheduleVersionDTO>() ?? new();
        }

        public async Task<List<CouresDTO>> GetCourses(string? userName, string? className, string? id)
        {
            string path = APIEndPoint.TempUrls.TempTimetableGetCourse
                .BuildUrl(
                    new Dictionary<string, string> { 
                        { "className", className ?? ""},
                        { "userName", userName  ?? ""},
                        { "coursesId", id  ?? ""}
                    }
                );
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.GET);

            return responseDTO.ConvertTo<List<CouresDTO>>() ?? [];
        }

        public async Task<CouresDTO> StoreCourses(CreateTempCourseRequest request)
        {
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempTimetableCreateCourse, ApiType.POST, request);
            if (responseDTO.StatusCode == HttpStatusCode.OK)
            {
                return responseDTO.ConvertTo<CouresDTO>() ?? new();
            }
            
            
            return new();
            
        }

        public async Task<ExamDTO> StoreExam(QuizDetail request)
        {
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempExam, ApiType.POST, request);
            if (responseDTO.StatusCode == HttpStatusCode.OK)
            {
                return responseDTO.ConvertTo<ExamDTO>() ?? new();
            }
            
            
            return new();
            
        }

        public async Task<List<ExamDTO>> GetExams(string major, int page = 0, int limit = 10)
        {
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempExamMajor.
                BuildUrl( new Dictionary<string, string>
                {
                      {"major", major },
                    {"page", page.ToString() },
                    {"limit", limit.ToString() }
                })
                , ApiType.GET);
            if (responseDTO.StatusCode == HttpStatusCode.OK)
            {
                return responseDTO.ConvertTo<List<ExamDTO>>() ?? [];
            }


            return [];

        }
   
        public async Task<ExamDTO?> GetExam(string id)
        {
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempExam.
                BuildUrl(new Dictionary<string, string>
                {
                      {"id", id }
                })
                , ApiType.GET);
            if (responseDTO.StatusCode == HttpStatusCode.OK)
            {
                return responseDTO.ConvertTo<ExamDTO>() ?? null;
            }


            return null;

        }
        public async Task<Question[]> GetExamQuestions(string id)
        {
            ResponseDTO detailResponseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempExamQuestions.
                BuildUrl(new Dictionary<string, string>
                {
                        {"id", id }
                })
                , ApiType.GET);
            if (detailResponseDTO.StatusCode == HttpStatusCode.OK)
            {
                Question[] questions = detailResponseDTO.ConvertTo<Question[]>() ?? [];
                return questions;
            }
                    
            return [];

        }

        public async Task<TimetableOutputDTO> UpdateTimetable(UpdateTimetableRequest request)
        {
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempTimetableTimetable, ApiType.PUT, request);
            if (responseDTO.StatusCode == HttpStatusCode.OK)
            {
                return responseDTO.ConvertTo<TimetableOutputDTO>() ?? new();
            }
            return new();
        }

    }
}
