using Auth_API.Src.Models;
using Auth_API.Src.Services.Identity;
using Auth_API.Src.Services.Postcode;
using Auth_API.Src.Services.TempUser;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Helpers;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests.LoginRequest;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Auth_API.Src.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService,
        //ITempUserRegister tempUserRegister,
        IPostcodeService postcodeService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        //private readonly ITempUserRegister _tempUserRegister = tempUserRegister;
        private readonly IPostcodeService _postcodeService = postcodeService;

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(RegisterRequest request)
        {
            //string userAgent = Request.Headers.UserAgent.ToString();
            string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
            if (ipAddress == null)
                return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));


            ResponseDTO SignUpResponse = await _authService.SignUp(request);
            DataSignUp? SignUpResult = SignUpResponse.ConvertTo<DataSignUp>();

            if (SignUpResult is not { Result.Succeeded: true } || SignUpResult.User is null)
                return Unauthorized(new SignInResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.UserAlreadyExists)
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
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(LoginByUserNameRequest request)
        {
            string userAgent = Request.Headers.UserAgent.ToString();
            string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
            if (ipAddress == null)
                return BadRequest(LocalValue.Get(KeyStore.LoginNotAllowed));
            ResponseDTO result = await _authService.SignIn(request);
            if (result.StatusCode == HttpStatusCode.BadRequest) 
                return BadRequest(result.Message);
            DataSignIn? dataSignIn = result.ConvertTo<DataSignIn>();
            if (dataSignIn != null && dataSignIn.Result != null && dataSignIn.User != null)
            {
                AUserDTO user = dataSignIn.User;
                SignInResultModel signInResult = dataSignIn.Result;
                if (signInResult.Succeeded)
                {
                    ResponseDTO TokenResult = await _authService.TokenSubcriber(user.Id.ToString());
                    TokenResponseDTO? tokenResponse = TokenResult.ConvertTo<TokenResponseDTO>();
                    return tokenResponse != null ? Ok(new SignInResponse
                    {
                        RequiresTwoFactor = signInResult.RequiresTwoFactor,
                        Message = TokenResult.Message,
                        MaskEmail = user.Email,
                        UserToken = tokenResponse
                    }) : BadRequest(TokenResult.Message);
                }
                if (signInResult.RequiresTwoFactor)
                {
                    ResponseDTO resultSendEmail = await _postcodeService.SendVerificationEmail(
                        user.Email,
                        user.UserName,
                        userAgent,
                        ipAddress
                    );
                    if (resultSendEmail.StatusCode == HttpStatusCode.OK)
                        return Ok(
                            new SignInResponse
                            {
                                RequiresTwoFactor = signInResult.RequiresTwoFactor,
                                Message = resultSendEmail.Message,
                            });
                }
                return dataSignIn.Result.IsLockedOut ? BadRequest(LocalValue.Get(KeyStore.AccountLocked)) : BadRequest(LocalValue.Get(KeyStore.LoginNotAllowed));
            }
            return BadRequest(result.Message);

        }

    }
}
