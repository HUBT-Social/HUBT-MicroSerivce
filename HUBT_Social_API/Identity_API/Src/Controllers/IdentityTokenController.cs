using Amazon.Runtime.Internal;
using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings;
using HUBT_Social_Identity_Service.Services;
using HUBT_Social_Identity_Service.Services.IdentityCustomeService;
using Identity_API.Src.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Claims;

namespace Identity_API.Src.Controllers
{
    [Route("api/identity/token")]
    [ApiController]
    
    public class IdentityTokenController(
        IHubtIdentityService<AUser, ARole, UserToken> identityService,
        IOptions<JwtSetting> options,
        IMapper mapper) : DataLayerController(mapper,options)
    {
        private readonly ITokenService<AUser,UserToken> _tokenService = identityService.TokenService;
        private readonly IUserService<AUser, ARole> _userService = identityService.UserService;
        [HttpPost("refreshToken")]
        public async Task<IActionResult> IsValidateToken([FromBody] string refreshToken)
        {
            string? accessToken = Request.Headers.ExtractBearerToken();
            if (accessToken != null)
            {
                TokenResponseDTO? result = await _tokenService.ValidateTokens(accessToken, refreshToken);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    Console.WriteLine("ValidateTokens is null");
                    return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));
                }
            }
            return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
        }
        [HttpPost]
        public async Task<IActionResult> GenerateToken([FromBody] string id)
        {
            AUser? user = await _userService.FindUserByIdAsync(id);
            if (user != null)
            {
                TokenResponseDTO? tokenResponseDTO = await _tokenService.GenerateTokenAsync(user);
                if (tokenResponseDTO != null)
                {
                    return Ok(tokenResponseDTO);
                }
            }
            return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteToken()
        {
            TokenInfoDTO? tokenInfo = Request.ExtractTokenInfo(_jwtSetting);
            if (tokenInfo != null)
            {
                AUser? user = tokenInfo.UserId != null
                    ? await _userService.FindUserByIdAsync(tokenInfo.UserId)
                    : null;

                if (user != null && await _tokenService.DeleteTokenAsync(user))
                    return Ok(LocalValue.Get(KeyStore.TokenDeleted));
            }

            return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
        }
    }
}
