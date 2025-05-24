using User_API.Src.UpdateUserRequest;
using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using Amazon.Runtime.Internal;
using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Core.Settings;

namespace User_API.Src.Service
{
    public class NotationService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), INotationService
    {
        public async Task SendNotation(string accessToken,AUserDTO userDTO)
        {
            string path = $"api/notation/send-to-one";
            SendMessageRequest request = new()
            {
                Title = LocalValue.Get(KeyStore.NotationSomeOneLoginYouAccountTitle),
                Body = $"{KeyStore.NotationSomeOneLoginYouAccountBody} {DateTime.UtcNow:HH:mm}",
                Token = userDTO.FCMToken ?? "",
                Type = "Warring",
            };
            try
            {
                HttpResponseMessage response = await SendActionResultRequestAsync(path, ApiType.POST, request, accessToken);
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }


    }
}
