using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;

namespace TempRegister_API.Src.Service
{
    public class OutService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IOutService
    {
        public async Task<ResponseDTO> GetCoureInfo(string courseName)
        {
            string path = $"/api/hubt/course/{courseName}";
            return await SendRequestAsync(path, HUBT_Social_Core.Settings.@enum.ApiType.GET);
        }

        public async Task<ResponseDTO> GetUserInClass(string className)
        {
            className = className.ToUpper();
            string path = $"/api/hubt/sinhvien/{className}";
            return await SendRequestAsync(path, HUBT_Social_Core.Settings.@enum.ApiType.GET);
        }
    }
}
