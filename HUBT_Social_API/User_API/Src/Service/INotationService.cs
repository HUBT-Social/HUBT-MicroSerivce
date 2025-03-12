using User_API.Src.UpdateUserRequest;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;

namespace User_API.Src.Service
{
    public interface INotationService : IBaseService
    {
        Task SendNotation(AUserDTO userDTO);

    }
}
