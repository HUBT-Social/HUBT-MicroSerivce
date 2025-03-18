using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Helpers;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests.LoginRequest;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using GatewayTest.Services.Identity;
using GatewayTest.Services.TempUser;
using GatewayTest.Services.Postcode;

namespace GatewayTest.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService,
        ITempUserRegister tempUserRegister,
        IPostcodeService postcodeService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ITempUserRegister _tempUserRegister = tempUserRegister;
        private readonly IPostcodeService _postcodeService = postcodeService;

        [HttpGet("auth/ping")]
        public async Task<bool> PingAuth()
        {
            var response = await _authService.Ping();
            return response.IsSuccessStatusCode;
        }
        [HttpGet("identity/ping")]
        public async Task<bool> PingPostcode()
        {
            var response = await _postcodeService.Ping();
            return response.IsSuccessStatusCode;
        }
        [HttpGet("temp/ping")]
        public async Task<bool> PingTempuser()
        {
            var response = await _tempUserRegister.Ping();
            return response.IsSuccessStatusCode;
        }

    }
}
