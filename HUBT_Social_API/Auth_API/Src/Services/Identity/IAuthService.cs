using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.LoginRequest;
using Microsoft.AspNetCore.Mvc;

namespace Auth_API.Src.Services.Identity
{
    public interface IAuthService : IBaseService
    {
        Task<ResponseDTO> SignIn(LoginByUserNameRequest request);
        Task<ResponseDTO> SignUp(RegisterRequest request);
        Task<ResponseDTO> TokenSubcriber(string userId);
        Task<AUserDTO?> IsUsed(RegisterRequest request);

    }
}
