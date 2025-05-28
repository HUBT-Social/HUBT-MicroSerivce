using Chat_API.Src.Interfaces;
using HUBT_Social_Base.Service;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using Chat_API.Src.Constants;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings.@enum;
using System.Net;
using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Core.Models.Requests;
using Amazon.Runtime.Internal;
using HUBT_Social_Core.Settings;

namespace Chat_API.Src.Services
{
    public class CourseService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), ICourseService
    {
        public async Task<List<CreateGroupByCourse>?> GetCourse(int page)
        {
            string path = APIEndPoint.TempUrls.TempCourseGet
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        {"page", page.ToString() }
                    }
                );
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET, null, null);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.ConvertTo<List<CreateGroupByCourse>>();
            }
            return null;
        }
        public async Task<bool> PutStatus(string courseId)
        {
            string path = APIEndPoint.TempUrls.TempCourseUpdateStatus
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        {"courseId", courseId }
                    }
                );
            ResponseDTO response = await SendRequestAsync(path, ApiType.PUT, null, null);
            if (response.StatusCode == HttpStatusCode.OK) return true;
            return false;
        }

    }
    
}

