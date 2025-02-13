using Amazon.Runtime.Internal.Transform;
using Auth_API.Src.Services.Identity;
using Auth_API.Src.Services.Postcode;
using Auth_API.Src.Services.TempUser;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Helpers;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Auth_API.Src.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthTokenController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("refresh-token")]
        public async Task<IActionResult> ActionResult([FromBody] string refreshToken)
        {
            string? token = Request.Headers.ExtractBearerToken();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            ResponseDTO result = await _authService.RefreshToken(token,refreshToken);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                TokenResponseDTO? tokenResponse = result.ConvertTo<TokenResponseDTO>();
                return tokenResponse != null ? Ok(tokenResponse) : BadRequest(LocalValue.Get(KeyStore.DataNotAllowNull));
            }
            return BadRequest(result.Message);
        }
        [HttpDelete("delete-token")]
        public async Task<IActionResult> DeleteToken()
        {
            string? token = Request.Headers.ExtractBearerToken();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            ResponseDTO result = await _authService.DeleteToken(token);
            return result.StatusCode switch
            {
                HttpStatusCode.OK => Ok(result.Message),
                HttpStatusCode.Unauthorized => Unauthorized(result.Message),
                _ => BadRequest(result.Message),
            };
        }

    }
}
