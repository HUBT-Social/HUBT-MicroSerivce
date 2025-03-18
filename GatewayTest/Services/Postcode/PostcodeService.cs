using GatewayTest.Services.Postcode;
using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Helpers;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.Settings.@enum;
using System.Net;

namespace GatewayTest.Services.Postcode
{
    public class PostcodeService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IPostcodeService
    {

        public async Task<HttpResponseMessage> Ping()
        {
            return await SendActionResultRequestAsync("ping", ApiType.GET);
        }
    }
}
