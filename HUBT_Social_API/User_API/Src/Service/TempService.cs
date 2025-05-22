using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Core.Models.DTOs;
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
        public async Task<TimetableOutputDTO> Get(string id)
        {
            string path = APIEndPoint.TempUrls.TempTimetable_GetTimetable
                .BuildUrl(
                    new Dictionary<string, string> { { "id", id } }
                );
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.GET);
            return responseDTO.ConvertTo<TimetableOutputDTO>() ?? new();
        }
        public async Task<List<TimetableOutputDTO>> GetList(string className)
        {
            string path = APIEndPoint.TempUrls.TempTimetable_GetTimetable
                .BuildUrl(
                    new Dictionary<string, string> { { "className", className } }
                );
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.GET);
            return responseDTO.ConvertTo<List<TimetableOutputDTO>>() ?? [];
        }

        public async Task<TimetableOutputDTO> StoreIn(TimetableOutputDTO request)
        {
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempTimetable_GetTimetable, ApiType.POST, request);
            return responseDTO.ConvertTo<TimetableOutputDTO>() ?? new();
        }

        public async Task<ClassScheduleVersionDTO> GetClassScheduleVersion(string className)
        {
            string path = APIEndPoint.TempUrls.TempTimetable_GetClassScheduleVersion
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
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempTimetable_CreateClassScheduleVersion, ApiType.POST, request);
            return responseDTO.ConvertTo<ClassScheduleVersionDTO>() ?? new();
        }
        public async Task<ClassScheduleVersionDTO> StoreClassScheduleVersion(ClassScheduleVersionDTO request)
        {   
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempTimetable_CreateClassScheduleVersion, ApiType.POST, request);
            return responseDTO.ConvertTo<ClassScheduleVersionDTO>() ?? new();
        }

        public async Task<List<CouresDTO>> GetCourses(string className)
        {
            string path = APIEndPoint.TempUrls.TempTimetable_GetCourse
                .BuildUrl(
                    new Dictionary<string, string> { { "className", className } }
                );
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.GET);
            
            return responseDTO.ConvertTo<List<CouresDTO>>() ?? [];

        }

        public async Task<CouresDTO> StoreCourses(CreateTempCourseRequest request)
        {
            ResponseDTO responseDTO = await SendRequestAsync(APIEndPoint.TempUrls.TempTimetable_CreateCourse, ApiType.POST, request);
            if (responseDTO.StatusCode == HttpStatusCode.OK)
            {
                return responseDTO.ConvertTo<CouresDTO>() ?? new();
            }
            else
            {
                throw new Exception($"Error: {responseDTO.Message}");
            }
        }
    }
}
