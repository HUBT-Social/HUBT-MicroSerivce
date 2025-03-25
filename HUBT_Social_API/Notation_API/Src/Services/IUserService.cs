using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;

namespace Notation_API.Src.Services
{
    public interface IUserService : IBaseService
    {
        Task<AUserDTO?> GetUserFCM(string accessToken);
        Task<string?> GetUserFCMFromId(string userId);
    }
}
