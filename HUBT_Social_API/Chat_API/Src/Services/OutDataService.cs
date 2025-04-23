using Chat_API.Src.Constants;
using Chat_API.Src.Interfaces;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using HUBT_Social_Base;
using HUBT_Social_Base.Service;

namespace Chat_API.Src.Services
{
    public class OutDataService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IOutDataService
    {
        public async Task<ResponseDTO> GetStudentsByClassName(string className)
        {
            string path = ChatApiEndpoints.OutService_GetUsersByClassName
                    .Replace("{{className}}", className);
            return await SendRequestAsync(path, ApiType.GET);
        }
    }
}
