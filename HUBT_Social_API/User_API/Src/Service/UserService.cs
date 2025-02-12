using User_API.Src.UpdateUserRequest;
using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;

namespace User_API.Src.Service
{
    public class UserService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IUserService
    {
        public async Task<ResponseDTO> GetUser()
        {
            string path = $"user";
            return await SendRequestAsync(path, ApiType.GET);
        }

        public Task<ResponseDTO> PromoteUserAccountAsync(string currentUserName, string targetUserName, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> UpdateAvatarUrlAsync(string userName, UpdateAvatarUrlRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> UpdateEmailAsync(string userName, UpdateEmailRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> VerifyCurrentPasswordAsync(string userName, CheckPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> UpdatePasswordAsync(string userName, UpdatePasswordRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> UpdateNameAsync(string userName, UpdateNameRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> UpdatePhoneNumberAsync(string userName, UpdatePhoneNumberRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> UpdateGenderAsync(string userName, UpdateGenderRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> UpdateDateOfBirthAsync(string userName, UpdateDateOfBornRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> AddInfoUser(string userName, AddInfoUserRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> EnableTwoFactor(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> DisableTwoFactor(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> GeneralUpdateAsync(string userName, GeneralUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> DeleteUserAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
