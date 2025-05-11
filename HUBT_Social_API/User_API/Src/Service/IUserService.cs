using User_API.Src.UpdateUserRequest;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;

namespace User_API.Src.Service
{
    public interface IUserService : IBaseService
    {
        Task<ResponseDTO> GetUser(string accessToken);
        Task<ResponseDTO> GetUserByRole(string roleName,int page);
        Task<ResponseDTO> FindUserByUserName(string accessToken, string userName);
        Task<ResponseDTO> PromoteUserAccountAsync(string accessToken, PromoteUserRequestDTO request);
        Task<ResponseDTO> UpdateAvatarUrlAsync(string accessToken, UpdateAvatarUrlRequest request);
        Task<ResponseDTO> UpdateNameAsync(string accessToken, UpdateNameRequest request);
        Task<ResponseDTO> UpdatePhoneNumberAsync(string accessToken, UpdatePhoneNumberRequest request);
        Task<ResponseDTO> UpdateGenderAsync(string accessToken, UpdateGenderRequest request);
        Task<ResponseDTO> UpdateFCM(string accessToken, string FCMKey, string deviceId);
        Task<ResponseDTO> UpdateBio(string accessToken, string bio);
        Task<ResponseDTO> UpdateDateOfBirthAsync(string accessToken, UpdateDateOfBornRequest request);
        Task<ResponseDTO> UpdateAddClassName(string accessToken, StudentClassName studentClassName);
        Task<ResponseDTO> AddInfoUser(string accessToken, AddInfoUserRequest request);
        Task<ResponseDTO> UpdateUserAdmin(string accessToken, AUserDTO user);

        Task<ResponseDTO> EnableTwoFactor(string accessToken);

        Task<ResponseDTO> DisableTwoFactor(string accessToken);

        Task<ResponseDTO> DeleteUserAsync(string accessToken);
    }
}
