using AutoMapper;
using Hangfire.Mongo.Dto;
using HUBT_Social_Base;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
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
        [HttpGet("userAll")]
        [AllowAnonymous]
        public IActionResult GetUserAll()
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
        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var tokenInfo = Request.ExtractTokenInfo(_jwtSetting);
            if (tokenInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            AUser? user = await _identityService.FindUserByIdAsync(tokenInfo.UserId);

            if (user != null)
            {
                AUserDTO aUserDTO = _mapper.Map<AUserDTO>(user);
                return Ok(aUserDTO);
            }

            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

        }
        [HttpGet("user/get")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUser([FromQuery] string? email, [FromQuery] string? userName)
        {
            AUserDTO? userDTO = null;

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(userName))
            {
                AUser? userByEmail = await _identityService.FindUserByEmailAsync(email);
                AUser? userByUserName = await _identityService.FindUserByUserNameAsync(userName);

                if (userByEmail != null && userByUserName != null)
                {
                    if (userByEmail.Id == userByUserName.Id)
                    {
                        userDTO = _mapper.Map<AUserDTO>(userByEmail);
                    }
                    else
                    {
                        return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
                    }
                }
                else if (userByEmail != null)
                {
                    userDTO = _mapper.Map<AUserDTO>(userByEmail);
                }
                else if (userByUserName != null)
                {
                    userDTO = _mapper.Map<AUserDTO>(userByUserName);
                }
            }
            else if (!string.IsNullOrEmpty(email))
            {
                AUser? userByEmail = await _identityService.FindUserByEmailAsync(email);
                userDTO = userByEmail != null ? _mapper.Map<AUserDTO>(userByEmail) : null;
            }
            else if (!string.IsNullOrEmpty(userName))
            {
                AUser? userByUserName = await _identityService.FindUserByUserNameAsync(userName);
                userDTO = userByUserName != null ? _mapper.Map<AUserDTO>(userByUserName) : null;
            }

            return Ok(userDTO);
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

                if (!string.IsNullOrEmpty(updateRequest.AvataUrl))
                    user.AvataUrl = updateRequest.AvataUrl;
                
                if (!string.IsNullOrEmpty(updateRequest.Status))
                    user.Status = updateRequest.Status;

                if (!string.IsNullOrEmpty(updateRequest.FCMToken))
                    user.FCMToken = updateRequest.FCMToken;
                
                if (updateRequest.Gender != null)
                    user.Gender = updateRequest.Gender.Value;

                if (updateRequest.DateOfBirth != null)
                    user.DateOfBirth = updateRequest.DateOfBirth.Value;

                if (updateRequest.EnableTwoFactor != null)
                    user.TwoFactorEnabled = updateRequest.EnableTwoFactor.Value;
                
                if (await _identityService.UpdateUserAsync(user))
                    return Ok(LocalValue.Get(KeyStore.UserInfoUpdatedSuccess));
            }
            catch (Exception)
            {
                return BadRequest(LocalValue.Get(KeyStore.GeneralUpdateError));
            }
            return BadRequest(LocalValue.Get(KeyStore.GeneralUpdateError));
        }
        [HttpDelete("delete-user")]
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
        [HttpPut("user/change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromQuery] string userName,[FromBody] UpdatePasswordRequestDTO changePasswordDTO)
        {
            bool result = await _identityService.UpdatePasswordAsync(userName, changePasswordDTO);
            return result ? Ok(LocalValue.Get(KeyStore.PasswordUpdated)) : BadRequest(LocalValue.Get(KeyStore.PasswordUpdateError));
        }

        [HttpPost("promote")]
        public async Task<IActionResult> Promote([FromBody] PromoteUserRequestDTO request)
        {
            TokenInfoDTO? tokenInfo = Request.ExtractTokenInfo(_jwtSetting);
            AUser? user =
                tokenInfo != null ?
                await _identityService.FindUserByIdAsync(tokenInfo.UserId) : null;
            if (user?.UserName != null && await _identityService.PromoteUserAccountAsync(user.UserName,request.UserName,request.RoleName))
            {
                return Ok(LocalValue.Get(KeyStore.UserInfoUpdatedSuccess));
            }
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
        }
    }
}
