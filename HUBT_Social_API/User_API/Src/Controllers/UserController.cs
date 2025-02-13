using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
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
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _identityService = userService;
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? name, [FromQuery] string? id,[FromQuery] string? email)
        {
            ResponseDTO result = await _identityService.GetUser();
            List<AUserDTO>? userDTO = result.ConvertTo<List<AUserDTO>>();
            if (userDTO != null && result.StatusCode == HttpStatusCode.OK)
            {

                if (!string.IsNullOrEmpty(name))
                {
                    userDTO = userDTO.Where(user => user.UserName == name).ToList();
                }
                if (!string.IsNullOrEmpty(email))
                {
                    userDTO = userDTO.Where(user => user.Email == email).ToList();
                }
                if (!string.IsNullOrEmpty(id))
                {
                    userDTO = userDTO.Where(user => user.Id.ToString() == id).ToList();
                }
                return Ok(userDTO);

            }
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
        public Task<IActionResult> UpdateFcm([FromBody] string fcmtoken)
        {
            return HandleServiceResponse(() => 
            _identityService.UpdateFCM(Request.Headers.ExtractBearerToken()!,fcmtoken));
        }
        [HttpPut("update/status")]
        public Task<IActionResult> UpdateStatus([FromBody] string bio)
        {
            return HandleServiceResponse(() => 
            _identityService.UpdateBio(Request.Headers.ExtractBearerToken()!,bio));
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
