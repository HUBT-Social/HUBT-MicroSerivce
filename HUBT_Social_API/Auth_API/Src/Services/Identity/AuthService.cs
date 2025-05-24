using Amazon.Runtime.Internal.Transform;
using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.ASP_Extensions;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.LoginRequest;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.Settings.@enum;
using System.Net;
using System.Xml.Linq;

namespace Auth_API.Src.Services.Identity
{
    public class AuthService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IAuthService
    {
        public async Task<ResponseDTO> SignIn(LoginByUserNameRequest request)
        {
            return await SendRequestAsync(APIEndPoint.IdentityUrls.PostLogin, ApiType.POST, request);
        }

        public async Task<ResponseDTO> SignUp(RegisterRequest request)
        {
            return await SendRequestAsync(APIEndPoint.IdentityUrls.PostSignUp, ApiType.POST, request);
        }
       
        public async Task<AUserDTO?> IsUsed(RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.UserName))
            {
                return null;
            }
            string path = APIEndPoint.IdentityUrls.GetUserFromEUI
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        { "email" , request.Email },
                        { "userName" , request.UserName }
                    }
                );
            ResponseDTO result = await SendRequestAsync(path, ApiType.GET);

            if (result.StatusCode == HttpStatusCode.OK && result.ConvertTo<AUserDTO>() is AUserDTO user)
            {
                return user;
            }

            return null;
        }
        public async Task<ResponseDTO> Forgotpassword(string userName, UpdatePasswordRequestDTO request)
        {
            string path = APIEndPoint.IdentityUrls.PutChangePassword
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        {"userName",userName}
                    }
                );
            return await SendRequestAsync(path, ApiType.PUT,request);
        }

        public async Task<ResponseDTO> TokenSubcriber(string userId)
        {
            return await SendRequestAsync(APIEndPoint.IdentityUrls.PostGenerateToken, ApiType.POST, userId);
        }

        public Task<ResponseDTO> RefreshToken(string accessToken, string refreshToken)
        {
            return SendRequestAsync(APIEndPoint.IdentityUrls.PostRefreshToken, ApiType.POST, refreshToken, accessToken);
        }

        public Task<ResponseDTO> DeleteToken(string accessToken)
        {
            return SendRequestAsync(APIEndPoint.IdentityUrls.DeleteToken, ApiType.DELETE, null, accessToken);
        }

        public async Task<AUserDTO?> CurrentUser(string accessToken)
        {
            string path = $"user";
            ResponseDTO result = await SendRequestAsync(path, ApiType.GET, null, accessToken);
            if (result.StatusCode == HttpStatusCode.OK && result.ConvertTo<AUserDTO>() is AUserDTO user)
                return user;
            else
            
                return null;
            
        }
    }
}
