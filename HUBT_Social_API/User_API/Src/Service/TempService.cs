using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.ExamDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings.@enum;
using System.Net;

namespace User_API.Src.Service
{
    public class TempService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), ITempService
    {
        public async Task<TimetableOutputDTO> Get(string id)
        {
            ResponseDTO responseDTO = await SendRequestAsync($"temptimetable?id={id}", ApiType.GET);
            return responseDTO.ConvertTo<TimetableOutputDTO>() ?? new();
        }
        public async Task<List<TimetableOutputDTO>> GetList(string className)
        {
            ResponseDTO responseDTO = await SendRequestAsync($"temptimetable?className={className}", ApiType.GET);
            return responseDTO.ConvertTo<List<TimetableOutputDTO>>() ?? [];
        }

        public async Task<TimetableOutputDTO> StoreIn(TimetableOutputDTO request)
        {
            ResponseDTO responseDTO = await SendRequestAsync("temptimetable", ApiType.POST, request);
            return responseDTO.ConvertTo<TimetableOutputDTO>() ?? new();
        }

        public async Task<ClassScheduleVersionDTO> GetClassScheduleVersion(string className)
        {
            ResponseDTO responseDTO = await SendRequestAsync($"temptimetable/classscheduleversion?className={className}", ApiType.GET);
            return responseDTO.ConvertTo<ClassScheduleVersionDTO>() ?? new();
        }

        public async Task<ClassScheduleVersionDTO> StoreClassScheduleVersion(string className, DateTime expireTime)
        {
            ClassScheduleVersionDTO request = new()
            {
                ClassName = className,
                ExpireTime = expireTime
            };
            ResponseDTO responseDTO = await SendRequestAsync("temptimetable/classscheduleversion", ApiType.POST, request);
            return responseDTO.ConvertTo<ClassScheduleVersionDTO>() ?? new();
        }
        public async Task<ClassScheduleVersionDTO> StoreClassScheduleVersion(ClassScheduleVersionDTO request)
        {   
            ResponseDTO responseDTO = await SendRequestAsync("temptimetable/classscheduleversion", ApiType.POST, request);
            return responseDTO.ConvertTo<ClassScheduleVersionDTO>() ?? new();
        }

        public async Task<List<CouresDTO>> GetCourses(string className)
        {
            string path = $"temptimetable/courses?className={className}";
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.GET);
            
            return responseDTO.ConvertTo<List<CouresDTO>>() ?? [];

        }

        public async Task<CouresDTO> StoreCourses(CreateTempCourseRequest request)
        {
            string path = $"temptimetable/courses";
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.POST, request);
            if (responseDTO.StatusCode == HttpStatusCode.OK)
            {
                return responseDTO.ConvertTo<CouresDTO>() ?? new();
            }
            
            
            return new();
            
        }

        public async Task<ExamDTO> StoreExam(ExamDTO request)
        {
            string path = $"tempexam";
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.POST, request);
            if (responseDTO.StatusCode == HttpStatusCode.OK)
            {
                return responseDTO.ConvertTo<ExamDTO>() ?? new();
            }
            
            
            return new();
            
        }
    }
}
