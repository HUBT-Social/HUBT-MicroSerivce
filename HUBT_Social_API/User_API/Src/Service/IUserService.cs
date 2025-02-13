using User_API.Src.UpdateUserRequest;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;

namespace User_API.Src.Service
{
    public interface IUserService : IBaseService
    {
        Task<ResponseDTO> GetUser();
        Task<ResponseDTO> PromoteUserAccountAsync(string accessToken, PromoteUserRequestDTO request);
        Task<ResponseDTO> UpdateAvatarUrlAsync(string accessToken, UpdateAvatarUrlRequest request);
        Task<ResponseDTO> UpdateNameAsync(string accessToken, UpdateNameRequest request);
        Task<ResponseDTO> UpdatePhoneNumberAsync(string accessToken, UpdatePhoneNumberRequest request);
        Task<ResponseDTO> UpdateGenderAsync(string accessToken, UpdateGenderRequest request);
        Task<ResponseDTO> UpdateFCM(string accessToken, string FCMKey);
        Task<ResponseDTO> UpdateBio(string accessToken, string bio);
        Task<ResponseDTO> UpdateDateOfBirthAsync(string accessToken, UpdateDateOfBornRequest request);
        Task<ResponseDTO> AddInfoUser(string accessToken, AddInfoUserRequest request);

        Task<ResponseDTO> EnableTwoFactor(string accessToken);

        Task<ResponseDTO> DisableTwoFactor(string accessToken);

        Task<ResponseDTO> DeleteUserAsync(string accessToken);
    }
}
