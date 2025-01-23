using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings.@enum;

namespace Auth_API.Src.Services.TempUser
{
    public class TempUserRegister(IHttpService httpService, string basePath) : BaseService(httpService, basePath), ITempUserRegister
    {
        public async Task<ResponseDTO> Get(string email)
        {
            return await SendRequestAsync($"tempRegister?email={email}", ApiType.GET);
        }

        public async Task<ResponseDTO> StoreIn(RegisterRequest request)
        {
            return await SendRequestAsync("tempRegister", ApiType.POST, request);
        }
    }
}
