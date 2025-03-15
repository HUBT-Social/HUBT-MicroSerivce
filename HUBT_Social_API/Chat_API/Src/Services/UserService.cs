using HUBT_Social_Base.Service;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings.@enum;
using HUBT_Social_Base.ASP_Extentions;
using Chat_API.Src.Interfaces;

namespace Chat_API.Src.Services
{
    public class UserService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IUserService
    {
        public async Task<HttpResponseMessage> GetUserRequest(string accessToken)
        {
            string path = $"api/user";
            return await SendActionResultRequestAsync(path, ApiType.GET, null, accessToken);
        }
        public async Task<HttpResponseMessage> GetAllUser(string accessToken)
        {
            string path = $"api/user/userAll";
            return await SendActionResultRequestAsync(path, ApiType.GET, null, accessToken);
        }
        public async Task<AUserDTO?> GetUserById(string? userId,string accessToken)
        {
            if (userId == null) { return null; }
            HttpResponseMessage res = await GetAllUser(accessToken);
            List<AUserDTO>? users = await res.ConvertTo<List<AUserDTO>>();
            if(users == null || users.Count == 0) { return null; }
            AUserDTO? target = users.Where(u => u.Id.ToString() == userId).FirstOrDefault();
            return target;
        }
    }
}

