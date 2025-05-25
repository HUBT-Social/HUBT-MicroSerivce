using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests.Firebase;

namespace Notation_API.Src.Services
{
    public interface IUserService : IBaseService
    {
        Task<AUserDTO?> GetUserFCM(string accessToken);
        Task<string?> GetUserFCMFromId(string userId);
        Task<List<string>?> GetListFMCFromListUserName(List<string> request);
        Task<List<string>> GetListFMCFromCondition(ConditionRequest request);
    }
}
