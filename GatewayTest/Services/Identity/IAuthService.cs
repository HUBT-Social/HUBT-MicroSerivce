using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.LoginRequest;
using Microsoft.AspNetCore.Mvc;

namespace GatewayTest.Services.Identity
{
    public interface IAuthService : IBaseService
    {
        Task<HttpResponseMessage> Ping();


    }
}
