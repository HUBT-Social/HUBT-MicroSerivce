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

namespace Chat_API.Src.Services
{
    public class CourseService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), ICourseService
    {
        public async Task<List<CreateGroupByCourse>?> GetCourse(int page)
        {
            string path = ChatApiEndpoints.TempService_GetCourse
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        {"page", page }
                    }
                );
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET, null, null);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.ConvertTo<List<CreateGroupByCourse>>();
            }
            return null;
        }

    }
}

