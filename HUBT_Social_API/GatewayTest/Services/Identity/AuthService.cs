using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.LoginRequest;
using HUBT_Social_Core.Settings.@enum;
using System.Net;
using System.Xml.Linq;

namespace GatewayTest.Services.Identity
{
    public class AuthService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IAuthService
    {
        public async Task<HttpResponseMessage> Ping()
        {
            return await SendActionResultRequestAsync("ping", ApiType.GET);
        }
    }
}
