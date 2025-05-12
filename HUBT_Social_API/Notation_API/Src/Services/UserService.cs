using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using Amazon.Runtime.Internal;
using HUBT_Social_Base.ASP_Extentions;
using System.Collections.Generic;
using System.Net;

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
        public async Task<List<string>?> GetListFMCFromListUserName(List<string> request)
        {
            string queryString = string.Join("&", request.Select(u => $"userNames={Uri.EscapeDataString(u)}"));
            string path = "users-in-list-userName" + "?" + queryString;
            ResponseDTO response = await SendRequestAsync(path,ApiType.GET,null,null);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<AUserDTO>? users = response.ConvertTo<List<AUserDTO>>();

                return users?
                    .Where(u => u?.FCMToken != null)              // Lọc user có FCMToken
                    .Select(u => u!.FCMToken!)                    // Lấy FCMToken (non-null sau khi lọc)
                    .ToList();
            }
            return null;
        }
    }
}
