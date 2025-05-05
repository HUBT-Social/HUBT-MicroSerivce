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
            return await SendRequestAsync(KeyStore.IdentityUrls.Post_Login, ApiType.POST, request);
        }

        public async Task<ResponseDTO> SignUp(RegisterRequest request)
        {
            return await SendRequestAsync(KeyStore.IdentityUrls.Post_SignUp, ApiType.POST, request);
        }
       
        public async Task<AUserDTO?> IsUsed(RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.UserName))
            {
                return null;
            }
            string path = KeyStore.IdentityUrls.Get_User_From_EUI
                .BuildUrl(
                    new Dictionary<string, object>
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
            string path = KeyStore.IdentityUrls.Put_Change_Password
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        {"userName",userName}
                    }
                );
            return await SendRequestAsync(path, ApiType.PUT,request);
        }

        public async Task<ResponseDTO> TokenSubcriber(string userId)
        {
            return await SendRequestAsync(KeyStore.IdentityUrls.Post_Generate_Token, ApiType.POST, userId);
        }

        public Task<ResponseDTO> RefreshToken(string accessToken, string refreshToken)
        {
            return SendRequestAsync(KeyStore.IdentityUrls.Post_Refresh_Token, ApiType.POST, refreshToken, accessToken);
        }

        public Task<ResponseDTO> DeleteToken(string accessToken)
        {
            return SendRequestAsync(KeyStore.IdentityUrls.Delete_Token, ApiType.DELETE, null, accessToken);
        }
    }
}
