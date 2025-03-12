using HUBT_Social_API.Src.Features.Auth.Dtos.Request;
using HUBT_Social_API.Src.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net;
using User_API.Src.Service;
using User_API.Src.UpdateUserRequest;

namespace User_API.Src.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController(IUserService userService,INotationService notationService) : ControllerBase
    {
        private readonly IUserService _identityService = userService;
        private readonly INotationService _notationService = notationService; 
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string? accessToken =  Request.Headers.ExtractBearerToken();
            if (accessToken == null)
            {
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            }
            ResponseDTO result = await _identityService.GetUser(accessToken);
            AUserDTO? userDTO = result.ConvertTo<AUserDTO>();
            if (userDTO != null && result.StatusCode == HttpStatusCode.OK)
                return Ok(userDTO);

            
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(result.Message);
            }
            return BadRequest(result.Message);

        }
        private async Task<IActionResult> HandleServiceResponse(Func<Task<ResponseDTO>> serviceMethod)
        {
            ResponseDTO result = await serviceMethod();

            return result.StatusCode switch
            {
                HttpStatusCode.OK => Ok(result.Message),
                HttpStatusCode.Unauthorized => Unauthorized(result.Message),
                _ => BadRequest(result.Message),
            };
        }

        [HttpDelete("delete-user")]
        public Task<IActionResult> Delete()
        {
            return HandleServiceResponse(() =>
            _identityService.DeleteUserAsync(Request.Headers.ExtractBearerToken()!));
        }

        [HttpPut("update-avatar")]
        public Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarUrlRequest request)
        {
            return HandleServiceResponse(() =>
            _identityService.UpdateAvatarUrlAsync(Request.Headers.ExtractBearerToken()!, request));
        }

        [HttpPut("update/name")]
        public Task<IActionResult> UpdateName([FromBody] UpdateNameRequest request)
        {
            return HandleServiceResponse(() =>
            _identityService.UpdateNameAsync(Request.Headers.ExtractBearerToken()!, request));
        }
        [HttpPut("update/two-factor-enable")]
        public Task<IActionResult> EnableTwoFactor()
        {
            return HandleServiceResponse(() =>
            _identityService.EnableTwoFactor(Request.Headers.ExtractBearerToken()!));
        }
        [HttpPut("update/two-factor-disable")]
        public Task<IActionResult> DisbleTwoFactor()
        {
            return HandleServiceResponse(() =>
            _identityService.DisableTwoFactor(Request.Headers.ExtractBearerToken()!));
        }
        [HttpPut("update/fcm-token")]
        public async Task<IActionResult> UpdateFcm([FromBody] StoreFCMRequest fcmtoken)
        {
            string? accessToken = Request.Headers.ExtractBearerToken();
            if (accessToken == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            ResponseDTO result = await _identityService.GetUser(accessToken);
            AUserDTO? userDTO = result.ConvertTo<AUserDTO>();
            if (userDTO == null)
                return Unauthorized(LocalValue.Get(KeyStore.UserNotFound));

            if (userDTO.FCMToken != fcmtoken.FcmToken)
            {
                try
                {
                    await _notationService.SendNotation(accessToken,userDTO);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            var response = await _identityService.UpdateFCM(Request.Headers.ExtractBearerToken()!, fcmtoken.FcmToken);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => Ok(response.Message),
                HttpStatusCode.Unauthorized => Unauthorized(response.Message),
                _ => BadRequest(response.Message),
            };
        }
        [HttpPut("delete/fcm-token")]
        public Task<IActionResult> UpdateFcm()
        {
            return HandleServiceResponse(() =>
            _identityService.UpdateFCM(Request.Headers.ExtractBearerToken()!, ""));
        }
        [HttpPut("update/status")]
        public Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest bio)
        {
            return HandleServiceResponse(() => 
            _identityService.UpdateBio(Request.Headers.ExtractBearerToken()!,bio.Bio));
        }
        [HttpPut("add-info-user")]
        public Task<IActionResult> EnableTwoFactor([FromBody] AddInfoUserRequest request)
        {
            return HandleServiceResponse(() => 
            _identityService.AddInfoUser(Request.Headers.ExtractBearerToken()!,request));
        }
        [HttpPut("update/phone-number")]
        public Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumberRequest request)
        {
            return HandleServiceResponse(() => 
            _identityService.UpdatePhoneNumberAsync(Request.Headers.ExtractBearerToken()!, request));
        }
        [HttpPost("promote")]
        public Task<IActionResult> PromoteUser([FromBody] PromoteUserRequestDTO request)
        {
            return HandleServiceResponse(() => 
            _identityService.PromoteUserAccountAsync(Request.Headers.ExtractBearerToken()!, request));
        }
    }
}
