using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using Amazon.Runtime.Internal;
using HUBT_Social_Base.ASP_Extentions;

namespace Notation_API.Src.Services
{
    public class UserService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IUserService
    {
        public async Task<AUserDTO?> GetUserFCM(string accessToken)
        {
            string path = $"user";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET,null, accessToken);
            AUserDTO? aUserDTO = response.ConvertTo<AUserDTO>();
            return aUserDTO;
        }

        public async Task<string?> GetUserFCMFromId(string userId)
        {
            string path = $"user/get?userId={userId}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET,null, null);
            AUserDTO? aUserDTO = response.ConvertTo<AUserDTO>();
            return aUserDTO?.FCMToken;
        }
    }
}
