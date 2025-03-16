using Auth_API.Src.Models;
using Auth_API.Src.Services.Identity;
using Auth_API.Src.Services.Postcode;
using Auth_API.Src.Services.TempUser;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Helpers;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.LoginRequest;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth_API.Src.Controllers
{

    [Route("api/auth")]
    [ApiController]
    public class AuthValidationController(IAuthService authService,
        ITempUserRegister tempUserRegister,
        IPostcodeService postcodeService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ITempUserRegister _tempUserRegister = tempUserRegister;
        private readonly IPostcodeService _postcodeService = postcodeService;

        [HttpPost("sign-up/verify-otp")]
        public async Task<IActionResult> ConfirmCodeSignUp([FromBody] VerifyPostcodeRequest request)
        {
            string userAgent = Request.Headers.UserAgent.ToString();
            string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
            if (ipAddress == null) return BadRequest(
                new SignInResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.InvalidInformation)
                });

            PostCodeDTO? currentPostcode = await _postcodeService.GetCurrentPostCode(new()
            {
                IpAddress = ipAddress,
                UserAgent = userAgent
            });
            if (currentPostcode == null) return BadRequest(
                new SignInResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.InvalidInformation)
                });



            if (!ModelState.IsValid)
                return BadRequest(
                    new SignInResponse
                    {
                        RequiresTwoFactor = false,
                        Message = LocalValue.Get(KeyStore.InvalidInformation)
                    }
                );

            if (!Equals(request.postcode, currentPostcode.Code))
            {
                return Unauthorized(new SignInResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
                }
                );
            }
            ResponseDTO tempUserResponse = await _tempUserRegister.Get(currentPostcode.Email);
            TempUserDTO? tempUserDTO = tempUserResponse.ConvertTo<TempUserDTO>();
            if (tempUserDTO == null)
                return Unauthorized(
                     new SignInResponse
                     {
                         RequiresTwoFactor = false,
                         Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
                     }
                    );

            ResponseDTO SignUpResponse= await _authService.SignUp(new RegisterRequest
            {
                Email = tempUserDTO.Email,
                Password = tempUserDTO.Password,
                ConfirmPassword = tempUserDTO.Password,
                UserName = tempUserDTO.UserName
            });
            DataSignUp? SignUpResult = SignUpResponse.ConvertTo<DataSignUp>();

            if (SignUpResult is not { Result.Succeeded: true } || SignUpResult.User is null)
                return Unauthorized(new SignInResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
                });

            AUserDTO user = SignUpResult.User;
            ResponseDTO TokenResult = await _authService.TokenSubcriber(user.Id.ToString());
            TokenResponseDTO? tokenResponse = TokenResult.ConvertTo<TokenResponseDTO>();
            return tokenResponse != null ? Ok(new SignInResponse
            {
                RequiresTwoFactor = false,
                Message = TokenResult.Message,
                MaskEmail = user.Email,
                UserToken = tokenResponse
            }) : BadRequest(
            new SignInResponse
                {
                    RequiresTwoFactor = false,
                    Message = TokenResult.Message,
                }
            );
        }
        [HttpPost("sign-in/verify-otp")]
        public async Task<IActionResult> ConfirmCodeSignIn([FromBody] VerifyPostcodeRequest request)
        {
            string userAgent = Request.Headers.UserAgent.ToString();
            string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
            if (ipAddress == null) return BadRequest(
                new SignInResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.InvalidInformation)
                });
            PostCodeDTO? currentPostcode = await _postcodeService.GetCurrentPostCode(new()
            {
                IpAddress = ipAddress,
                UserAgent = userAgent
            });
            if (currentPostcode == null) return BadRequest(
                new SignInResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.InvalidInformation)
                });
            if (Equals(request.postcode, currentPostcode.Code))
            {
                AUserDTO? aUser = await _authService.IsUsed(new RegisterRequest
                {
                    Email = currentPostcode.Email
                });
                if (aUser == null) return Unauthorized(
                    new SignInResponse
                    {
                        RequiresTwoFactor = false,
                        Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
                    });
                ResponseDTO TokenResult = await _authService.TokenSubcriber(aUser.Id.ToString());
                TokenResponseDTO? tokenResponse = TokenResult.ConvertTo<TokenResponseDTO>();
                return tokenResponse != null ? Ok(new SignInResponse
                {
                    RequiresTwoFactor = false,
                    Message = TokenResult.Message,
                    MaskEmail = currentPostcode.Email,
                    UserToken = tokenResponse
                }) : BadRequest(
                    new SignInResponse
                    {
                        RequiresTwoFactor = false,
                        Message = TokenResult.Message,
                    });
            }
            return BadRequest(
                new SignInResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
                }
            );
        }
    }
}
