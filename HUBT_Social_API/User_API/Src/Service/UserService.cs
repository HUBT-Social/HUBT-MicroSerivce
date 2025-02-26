using User_API.Src.UpdateUserRequest;
using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using Amazon.Runtime.Internal;

namespace User_API.Src.Service
{
    public class UserService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IUserService
    {
        public async Task<ResponseDTO> GetUser()
        {
            string path = $"userAll";
            return await SendRequestAsync(path, ApiType.GET);
        }

        public async Task<ResponseDTO> PromoteUserAccountAsync(string accessToken, PromoteUserRequestDTO request)
        {
            string path = $"promote";
            return await SendRequestAsync(path, ApiType.POST, request, accessToken);
        }

        private async Task<ResponseDTO> UpdateUserAsync(string accessToken, Action<UpdateUserDTO> updateAction)
        {
            string path = "update-user";
            UpdateUserDTO updateRequest = new();
            updateAction(updateRequest);
            return await SendRequestAsync(path, ApiType.PUT, updateRequest, accessToken);
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
            string path = $"delete-user";
            return SendRequestAsync(path, ApiType.DELETE,null,accessToken);
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
            return UpdateUserAsync(accessToken, dto => {
                dto.Gender = request.Gender;
                dto.FirstName = request.FirstName;
                dto.LastName = request.LastName;
                dto.PhoneNumber = request.PhoneNumber;
                dto.DateOfBirth = request.DateOfBirth;
            });
        }
        public Task<ResponseDTO> UpdateFCM(string accessToken, string FCMKey)
        {
            return UpdateUserAsync(accessToken, dto => {
                dto.FCMToken= FCMKey;
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
    }
}
