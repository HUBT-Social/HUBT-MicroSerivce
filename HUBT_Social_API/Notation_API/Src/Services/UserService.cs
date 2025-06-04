using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Base.ASP_Extentions;
using System.Collections.Generic;
using System.Net;
using HUBT_Social_Core.Models.Requests.Firebase;

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
        public async Task<NotificationRecipients> GetNotificationRecipientsFromCondition(ConditionRequest request)
        {

            string path = "get-notification-recipient";
            if (request.SendAll)
            {
                path += "?SendAll=true";
            }
            else
            {
                var queryParams = new List<string>();
                if (request.ClassCodes?.Count > 0)
                {
                    queryParams.Add($"ClassCodes={Uri.EscapeDataString(string.Join(",", request.ClassCodes))}");
                }
                if (request.FacultyCodes?.Count > 0)
                {
                    queryParams.Add($"FacultyCodes={Uri.EscapeDataString(string.Join(",", request.FacultyCodes))}");
                }
                if (request.CourseCodes?.Count > 0)
                {
                    queryParams.Add($"CourseCodes={Uri.EscapeDataString(string.Join(",", request.CourseCodes))}");
                }
                if (request.UserNames?.Count > 0)
                {
                    queryParams.Add($"UserNames={Uri.EscapeDataString(string.Join(",", request.UserNames))}");
                }
                if (queryParams.Count > 0)
                {
                    path += $"?{string.Join("&", queryParams)}";
                }
            }
            if (request.IncludeEmails) { path += "&IncludeEmails=true"; }
            if (request.IncludePhoneNumbers) { path += "&IncludePhoneNumbers=true"; }
            if (request.IncludeFcmTokens) { path += "&IncludeFcmTokens=true"; }

            ResponseDTO? response = null;
            try
            {
                response = await SendRequestAsync(path, ApiType.GET, null, null);
            }
            catch
            {
                return new NotificationRecipients();
            }

            if (response?.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    NotificationRecipients? recipients = response.ConvertTo<NotificationRecipients>();
                    return recipients ?? new NotificationRecipients();
                }
                catch
                {
                    return new NotificationRecipients();
                }
            }

            return new NotificationRecipients();

        }
    }
}
