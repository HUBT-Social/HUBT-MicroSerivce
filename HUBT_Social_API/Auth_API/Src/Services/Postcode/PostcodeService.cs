using Amazon.Runtime.Internal.Transform;
using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Helpers;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.Settings.@enum;
using System.Net;

namespace Auth_API.Src.Services.Postcode
{
    public class PostcodeService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IPostcodeService
    {
        
        private async Task<ResponseDTO> CreatePostcodeAsync(CreatePostcodeRequest request)
        {
            return await SendRequestAsync(APIEndPoint.PostCodeUrls.PostCreatePostCode, ApiType.POST, request);
        }
        public async Task<PostCodeDTO?> GetCurrentPostCode(PostcodeRequest request)
        {
            string path = APIEndPoint.PostCodeUrls.GetCurrentPostCode
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        {"UserAgent",request.UserAgent },
                        {"IpAddress",request.IpAddress }
                    }
                );
            ResponseDTO? response = await SendRequestAsync(path,ApiType.GET);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                PostCodeDTO? postcode = response.ConvertTo<PostCodeDTO>();
                return postcode;
            } 
            return null;
        }
        private async Task<ResponseDTO> SendPostcodeAsync(EmailRequest request)
        {
            return await SendRequestAsync(APIEndPoint.PostCodeUrls.PostSendPostCode, ApiType.POST, request);
        }
        public async Task<ResponseDTO> SendVerificationEmail(string email, string userName, string userAgent, string ipAddress)
        {
            var createPostcodeRequest = new CreatePostcodeRequest
            {
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Receiver = email
            };

            var resultCreatePostcode = await CreatePostcodeAsync(createPostcodeRequest);
            var postCodeDTO = resultCreatePostcode.ConvertTo<PostCodeDTO>();

            if (postCodeDTO == null || resultCreatePostcode.StatusCode != HttpStatusCode.OK)
            {
                return resultCreatePostcode;
            }

            var emailRequest = new EmailRequest
            {
                Code = postCodeDTO.Code,
                Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject),
                ToEmail = email,
                FullName = userName,
                Device = userAgent,
                Location = await ServerHelper.GetLocationFromIpAsync(ipAddress),
                DateTime = ServerHelper.ConvertToCustomString(DateTime.UtcNow)
            };

            return await SendPostcodeAsync(emailRequest);
        }
    }
}
