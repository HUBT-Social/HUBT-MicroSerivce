using HUBT_Social_Base.Service;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings.@enum;
using HUBT_Social_Base.ASP_Extentions;
using Chat_API.Src.Interfaces;
using HUBT_Social_Core.Models.DTOs;
using Chat_API.Src.Constants;
using System.Collections.Generic;
using HUBT_Social_Core.ASP_Extensions;
using System.Net;
using HUBT_Social_Core.Settings;

namespace Chat_API.Src.Services
{
    public class UserService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IUserService
    {
        public async Task<ResponseDTO> GetUserRequest(string accessToken)
        {
            string path = KeyStore.IdentityUrls.Get_Current_User;
            return await SendRequestAsync(path, ApiType.GET, null, accessToken);
        }
        public async Task<ResponseDTO> GetAllUser(string accessToken)
        {
            string path = KeyStore.IdentityUrls.Get_All_User;
            return await SendRequestAsync(path, ApiType.GET, null, accessToken);
        }
        public async Task<List<AUserDTO>?> GetUsersByUserNames(List<string> request, string accessToken)
        {
            string queryString = string.Join("&", request.Select(u => $"userNames={Uri.EscapeDataString(u)}"));
            Console.WriteLine(queryString);
            string path = KeyStore.IdentityUrls.Get_User_In_List_User_Name + "?"+ queryString;
            ResponseDTO response = await SendRequestAsync(path,ApiType.GET, null, accessToken);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                return response.ConvertTo<List<AUserDTO>>();
            }
            return null;
        }

        public async Task<AUserDTO?> GetUserByUserName(string userName, string accessToken)
        {
            string path = KeyStore.IdentityUrls.Get_User_From_EUI
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        {"userName", userName }
                    }
                );
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET, null, accessToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.ConvertTo<AUserDTO>();
            }
            return null;
        }
    }
}

