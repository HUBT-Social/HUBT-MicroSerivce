using User_API.Src.UpdateUserRequest;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace User_API.Src.Service
{
    public interface IUserService : IBaseService
    {
        Task<ResponseDTO> GetUser();
        Task<ResponseDTO> PromoteUserAccountAsync(string currentUserName, string targetUserName, string roleName);
        Task<ResponseDTO> UpdateAvatarUrlAsync(string userName, UpdateAvatarUrlRequest request);
        Task<ResponseDTO> UpdateEmailAsync(string userName, UpdateEmailRequest request);
        Task<ResponseDTO> VerifyCurrentPasswordAsync(string userName, CheckPasswordRequest request);
        Task<ResponseDTO> UpdatePasswordAsync(string userName, UpdatePasswordRequest request);
        Task<ResponseDTO> UpdateNameAsync(string userName, UpdateNameRequest request);
        Task<ResponseDTO> UpdatePhoneNumberAsync(string userName, UpdatePhoneNumberRequest request);
        Task<ResponseDTO> UpdateGenderAsync(string userName, UpdateGenderRequest request);
        Task<ResponseDTO> UpdateDateOfBirthAsync(string userName, UpdateDateOfBornRequest request);
        Task<ResponseDTO> AddInfoUser(string userName, AddInfoUserRequest request);

        Task<ResponseDTO> EnableTwoFactor(string userName);

        Task<ResponseDTO> DisableTwoFactor(string userName);
        Task<ResponseDTO> GeneralUpdateAsync(string userName, GeneralUpdateRequest request);

        Task<ResponseDTO> DeleteUserAsync(string id);
    }
}
