using HUBT_Social_API.Src.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using HUBT_Social_Core;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Authorization;
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
    public class UserController(IUserService userService,INotationService notationService, IOutSourceService outSourceService, IHttpCloudService cloudService) : ControllerBase
    {
        private readonly IUserService _identityService = userService;
        private readonly INotationService _notationService = notationService; 
        private readonly IOutSourceService _outSourceService = outSourceService;
        private readonly IHttpCloudService _cloudService = cloudService;
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
            {
                return Ok(new
                {
                    AvatarUrl = userDTO.AvataUrl,
                    userDTO.UserName,
                    userDTO.FirstName,
                    userDTO.LastName,
                    userDTO.Gender,
                    userDTO.Email,
                    BirthDay = userDTO.DateOfBirth,
                    userDTO.PhoneNumber
                });
            }
                

            
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(result.Message);
            }
            return BadRequest(result.Message);

        }
        [HttpGet("get-user-by-role")]
        public async Task<IActionResult> GetUserByRole([FromQuery] string roleName, [FromQuery] int page = 0)
        {
            ResponseDTO result = await _identityService.GetUserByRole(roleName, page);
            ResponseUserRoleDTO? responseUserRoleDTO = result.ConvertTo<ResponseUserRoleDTO>();
            return Ok(new
            {
                users = responseUserRoleDTO.users,
                hasMore = responseUserRoleDTO.hasMore,
                message = responseUserRoleDTO.message
            });
        }
        [HttpGet("user-find")]
        public async Task<IActionResult> GetAllUser(string usename)
        {
            string? accessToken = Request.Headers.ExtractBearerToken();
            if (accessToken == null)
            {
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            }
            ResponseDTO result = await _identityService.FindUserByUserName(accessToken,usename);

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
        public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarRequest request)
        {
            if (request.File == null) return BadRequest("File is null");

            string? newUrl = await _cloudService.GetUrlFormFile(request.File);
            if (string.IsNullOrEmpty(newUrl)) return BadRequest("Update failed");

            var token = Request.Headers.ExtractBearerToken();
            if (string.IsNullOrEmpty(token)) return Unauthorized("Token missing");

            return await HandleServiceResponse(() =>
                _identityService.UpdateAvatarUrlAsync(token, newUrl));
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

            string accessToken = Request.Headers.ExtractBearerToken() ?? "";
            string userAgent = Request.Headers.UserAgent.ToString() ?? "";
            if (accessToken == string.Empty)
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

            var response = await _identityService.UpdateFCM(Request.Headers.ExtractBearerToken()!, fcmtoken.FcmToken,userAgent);
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
            string userAgent = Request.Headers.UserAgent.ToString() ?? "";

            return HandleServiceResponse(() =>
            _identityService.UpdateFCM(Request.Headers.ExtractBearerToken()!, "",userAgent));
        }
        [HttpPut("update/status")]
        public Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest bio)
        {
            return HandleServiceResponse(() => 
            _identityService.UpdateBio(Request.Headers.ExtractBearerToken()!,bio.Bio));
        }
        [HttpPut("update-user-admin")]
        public Task<IActionResult> UpdateUserAdmin([FromBody] AUserDTO request)
        {
            return HandleServiceResponse(() =>
            _identityService.UpdateUserAdmin(Request.Headers.ExtractBearerToken()!, request));
        }
        [HttpPut("add-info-user")]
        public Task<IActionResult> EnableTwoFactor([FromBody] AddInfoUserRequest request)
        {
            return HandleServiceResponse(() => 
            _identityService.AddInfoUser(Request.Headers.ExtractBearerToken()!,request));
        }
        [HttpPut("add-className")]
        public async Task<IActionResult> UpdateAddClassName([FromBody] StudentClassName request)
        {
            string? accessToken = Request.Headers.ExtractBearerToken();
            if (accessToken == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.ClassName))
            {
                return BadRequest("Request must be not null.");
            }
            try { 

                await _identityService.UpdateAddClassName(accessToken, request);   
                return Ok();
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
            
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
