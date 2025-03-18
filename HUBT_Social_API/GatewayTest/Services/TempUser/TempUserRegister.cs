using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings.@enum;

namespace GatewayTest.Services.TempUser
{
    public class TempUserRegister(IHttpService httpService, string basePath) : BaseService(httpService, basePath), ITempUserRegister
    {
        public async Task<HttpResponseMessage> Ping()
        {
            return await SendActionResultRequestAsync("ping", ApiType.GET);
        }
    }
}
