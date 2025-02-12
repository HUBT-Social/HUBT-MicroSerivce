using Amazon.Runtime.Internal;
using Auth_API.Src.Services.Identity;
using Auth_API.Src.Services.Postcode;
using Auth_API.Src.Services.TempUser;
using HUBT_Social_Base.Helpers;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.LoginRequest;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net;

namespace Auth_API.Src.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthForgotPassword(IAuthService authService,
        IPostcodeService postcodeService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IPostcodeService _postcodeService = postcodeService;
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string identifier)
        {
            string userAgent = Request.Headers.UserAgent.ToString();
            string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
            if (ipAddress == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            RegisterRequest registerRequest = new()
            {
                Email = identifier,
                UserName = identifier
            };
            AUserDTO? user = await _authService.IsUsed(registerRequest);
            if (user == null) return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

            var result = await _postcodeService.SendVerificationEmail(
                user.Email,
                user.UserName,
                userAgent,
                ipAddress
            );
            return result.StatusCode == HttpStatusCode.OK
                ? Ok(new { result.Message, user.Email })
                : BadRequest(result.Message);

        }
        [HttpPost("forgot-password/password-verification")]
        public async Task<IActionResult> ConfirmCodeForgotPassword([FromBody] string code)
        {
            string userAgent = Request.Headers.UserAgent.ToString();
            string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
            if (ipAddress == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            PostCodeDTO? currentPostcode = await _postcodeService.GetCurrentPostCode(new()
            {
                IpAddress = ipAddress,
                UserAgent = userAgent
            });
            if (currentPostcode == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));

            if (!Equals(code, currentPostcode.Code))
            {
                return Ok(LocalValue.Get(KeyStore.OtpVerificationSuccess));
            }
            return Unauthorized(LocalValue.Get(KeyStore.InvalidInformation));

        }
        [HttpPut("forgot-password/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordRequestDTO request)
        {
            string userAgent = Request.Headers.UserAgent.ToString();
            string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
            if (ipAddress == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            if (!ModelState.IsValid) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            PostCodeDTO? currentPostcode = await _postcodeService.GetCurrentPostCode(new()
            {
                IpAddress = ipAddress,
                UserAgent = userAgent
            });
            if (currentPostcode == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));

            AUserDTO? user = await _authService.IsUsed(new RegisterRequest
            {
                Email = currentPostcode.Email,
                UserName = currentPostcode.Email
            });
            if (user == null) return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

            var result = await _authService.Forgotpassword(user.UserName, request);
            return result.StatusCode == HttpStatusCode.OK
                ? Ok(result.Message)
                : BadRequest(result.Message);
        }
    }
}
