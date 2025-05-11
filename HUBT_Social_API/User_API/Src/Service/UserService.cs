using User_API.Src.UpdateUserRequest;
using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using Amazon.Runtime.Internal;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.ASP_Extensions;
using Amazon.Runtime.Internal.Transform;
using MongoDB.Driver.Core.Operations;
using HUBT_Social_Core.Models.DTOs.UserDTO;

namespace User_API.Src.Service
{
    public class UserService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IUserService
    {
        public async Task<ResponseDTO> GetUser(string accessToken)
        {
            return await SendRequestAsync(KeyStore.IdentityUrls.Get_Current_User, ApiType.GET,null,accessToken);
        }
        public async Task<ResponseDTO> GetUserByRole(string roleName,int page)
        {
            string path = KeyStore.IdentityUrls.Get_User_From_RoleName
                .BuildUrl(
                    new Dictionary<string, object> { { "roleName", roleName },{ "page", page } }
                );
            return await SendRequestAsync(path, ApiType.GET, null, null);
        }
        public async Task<ResponseDTO> FindUserByUserName(string accessToken,string username)
        {
            string path = KeyStore.IdentityUrls.Get_User_From_EUI
                .BuildUrl(
                    new Dictionary<string, object> { { "username", username } }
                );
            return await SendRequestAsync(path, ApiType.GET, null, accessToken);
        }

        public async Task<ResponseDTO> PromoteUserAccountAsync(string accessToken, PromoteUserRequestDTO request)
        {
            return await SendRequestAsync(KeyStore.IdentityUrls.Post_Promote_Role, ApiType.POST, request, accessToken);
        }

        private async Task<ResponseDTO> UpdateUserAsync(string accessToken, Action<UpdateUserDTO> updateAction)
        {
            UpdateUserDTO updateRequest = new();
            updateAction(updateRequest);
            return await SendRequestAsync(KeyStore.IdentityUrls.Put_Update_User, ApiType.PUT, updateRequest, accessToken);
        }

        public Task<ResponseDTO> UpdateAvatarUrlAsync(string accessToken, UpdateAvatarUrlRequest request)
        {
            return UpdateUserAsync(accessToken, dto => dto.AvataUrl = request.AvatarUrl);
        }

        public Task<ResponseDTO> UpdateNameAsync(string accessToken, UpdateNameRequest request)
        {
            return UpdateUserAsync(accessToken, dto =>
            {
                dto.FirstName = request.FirstName;
                dto.LastName = request.LastName;
            });
        }

        public Task<ResponseDTO> DeleteUserAsync(string accessToken)
        {
            return SendRequestAsync(KeyStore.IdentityUrls.Delete_User, ApiType.DELETE,null,accessToken);
        }

        public Task<ResponseDTO> UpdatePhoneNumberAsync(string accessToken, UpdatePhoneNumberRequest request)
        {
            return UpdateUserAsync(accessToken, dto =>
            {
                dto.PhoneNumber = request.PhoneNumber;
            });
        }

        public Task<ResponseDTO> UpdateGenderAsync(string accessToken, UpdateGenderRequest request)
        {
            return UpdateUserAsync(accessToken, dto =>
            {
                dto.Gender = request.Gender;
            });
        }

        public Task<ResponseDTO> UpdateDateOfBirthAsync(string accessToken, UpdateDateOfBornRequest request)
        {
            return UpdateUserAsync(accessToken, dto =>
            {
                dto.DateOfBirth = request.DateOfBirth;
            });
        }

        public Task<ResponseDTO> AddInfoUser(string accessToken, AddInfoUserRequest request)
        {
            string avatarDefaultByGender = LocalValue.Get(KeyStore.GetRandomAvatarDefault(request.Gender));
            return UpdateUserAsync(accessToken, dto => {
                dto.Gender = request.Gender;
                dto.FirstName = request.FirstName;
                dto.LastName = request.LastName;
                dto.PhoneNumber = request.PhoneNumber;
                dto.DateOfBirth = request.DateOfBirth;
                dto.AvataUrl = avatarDefaultByGender;
            });
        }
        public Task<ResponseDTO> UpdateFCM(string accessToken, string FCMKey, string deviceId)
        {
            return UpdateUserAsync(accessToken, dto => {
                dto.FCMToken= FCMKey;
                dto.DeviceId = deviceId;
            });
        }
        public Task<ResponseDTO> UpdateBio(string accessToken, string bio)
        {
            return UpdateUserAsync(accessToken, dto => {
                dto.Status= bio;
            });
        }

        public Task<ResponseDTO> EnableTwoFactor(string accessToken)
        {
            return UpdateUserAsync(accessToken, dto =>
            {
                dto.EnableTwoFactor = true;
            });
        }

        public Task<ResponseDTO> DisableTwoFactor(string accessToken)
        {
            return UpdateUserAsync(accessToken, dto =>
            {
                dto.EnableTwoFactor = false;
            });
        }

        public async Task<ResponseDTO> UpdateUserAdmin(string accessToken, AUserDTO user)
        {
            return await SendRequestAsync(KeyStore.IdentityUrls.Put_Update_User_Admin, ApiType.PUT, user, accessToken);
        }

        public async Task<ResponseDTO> UpdateAddClassName(string accessToken,StudentClassName studentClassName)
        {
            return await SendRequestAsync(KeyStore.IdentityUrls.Put_Update_User_ClassName, ApiType.PUT, studentClassName, accessToken);
        }
    }
}
