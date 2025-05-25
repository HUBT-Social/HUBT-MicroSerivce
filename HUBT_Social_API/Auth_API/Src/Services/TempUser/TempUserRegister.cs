using Amazon.Runtime.Internal.Transform;
using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.Settings.@enum;

namespace Auth_API.Src.Services.TempUser
{
    public class TempUserRegister(IHttpService httpService, string basePath) : BaseService(httpService, basePath), ITempUserRegister
    {
        public async Task<ResponseDTO> Get(string email)
        {
            string path = APIEndPoint.TempUrls.TempRegister_GetTempUser
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        {"email",email }
                    }
                );
            return await SendRequestAsync(path, ApiType.GET);
        }

        public async Task<ResponseDTO> StoreIn(RegisterRequest request)
        {
            return await SendRequestAsync(APIEndPoint.TempUrls.TempRegister_StoreTempUser, ApiType.POST, request);
        }
    }
}
