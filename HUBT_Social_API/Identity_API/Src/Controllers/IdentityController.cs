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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Core.Operations;
using System.Xml.Linq;

namespace Identity_API.Src.Controllers
{
    [Route("api/identity")]
    [ApiController]
    [Authorize]
    public class IdentityController(IHubtIdentityService<AUser, ARole> identityService, IMapper mapper, IOptions<JwtSetting> options) : DataLayerController(mapper, options)
    {
        private readonly IUserService<AUser, ARole> _identityService = identityService.UserService;
        [HttpGet("user")]
        [AllowAnonymous]
        public IActionResult GetUser()
        {

            List<AUser>? listUser = _identityService.GetAll();

            if (listUser == null) return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

            if (listUser.Count > 0)
            {
                var userDTOs = listUser.Select(user => _mapper.Map<AUserDTO>(user)).ToList();

                return Ok(userDTOs);
                
            }
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

        }
        [HttpGet("user/get")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUser([FromQuery] string? email, [FromQuery] string? userName)
        {
            AUserDTO? usedDTO = null;

            if (!string.IsNullOrEmpty(email))
            {
                AUser? used = await _identityService.FindUserByEmailAsync(email);
                usedDTO = used != null ? _mapper.Map<AUserDTO>(used) : null;
            }

            if (usedDTO == null && !string.IsNullOrEmpty(userName))
            {
                AUser? used = await _identityService.FindUserByUserNameAsync(userName);
                usedDTO = used != null ? _mapper.Map<AUserDTO>(used) : null;
            }

            return Ok(usedDTO);
        }
        [HttpPut("update-user")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO updateRequest)
        {

            var tokenInfo = Request.ExtractTokenInfo(_jwtSetting);
            if (tokenInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            var user = await _identityService.FindUserByIdAsync(tokenInfo.UserId);
            if (user == null)
                return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
            try
            {
                if (!string.IsNullOrEmpty(updateRequest.FirstName))
                    user.FirstName = updateRequest.FirstName;

                if (!string.IsNullOrEmpty(updateRequest.LastName))
                    user.LastName = updateRequest.LastName;

                if (!string.IsNullOrEmpty(updateRequest.Email))
                    user.Email = updateRequest.Email;

                if (!string.IsNullOrEmpty(updateRequest.PhoneNumber))
                    user.PhoneNumber = updateRequest.PhoneNumber;

                if (updateRequest.Gender != null)
                    user.Gender = updateRequest.Gender.Value;

                if (updateRequest.DateOfBirth != null)
                    user.DateOfBirth = updateRequest.DateOfBirth.Value;
                
                if (updateRequest.EnableTwoFactor != null)
                    user.TwoFactorEnabled = updateRequest.EnableTwoFactor.Value;

                if (await _identityService.UpdateUserAsync(user))
                    return Ok(LocalValue.Get(KeyStore.GeneralUpdateSuccess));
            }
            catch (Exception)
            {
                return BadRequest(LocalValue.Get(KeyStore.GeneralUpdateError));
            }
            return BadRequest(LocalValue.Get(KeyStore.GeneralUpdateError));
        }
        [HttpDelete("Delete-user")]
        public async Task<IActionResult> Delete()
        {
            TokenInfoDTO? tokenInfo = Request.ExtractTokenInfo(_jwtSetting);
            AUser? user = 
                tokenInfo != null ? 
                await _identityService.FindUserByIdAsync(tokenInfo.UserId) : null;
            if  (user != null && await _identityService.DeleteUserAsync(user))
            {
                return Ok(user);
            }
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
        }

    }
}
