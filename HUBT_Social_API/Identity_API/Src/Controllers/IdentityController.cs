using AutoMapper;
using Hangfire.Mongo.Dto;
using HUBT_Social_Base;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings;
using HUBT_Social_Identity_Service.Services;
using HUBT_Social_Identity_Service.Services.IdentityCustomeService;
using HUBT_Social_MongoDb_Service.Services;
using Identity_API.Src.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
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
        private readonly IMongoService<AUser> _aUserService;
        
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
        [HttpGet("user-by-role")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTeacher([FromQuery] string roleName, [FromQuery] int page = 0)
        {

            var response = await _identityService.GetUserByRole(roleName, page);

            if (response.Item1.Count > 0)
            {
                var userDTOs = response.Item1.Select(user => {
                    AUserDTO u = _mapper.Map<AUserDTO>(user);
                    u.Status = string.IsNullOrEmpty(u.FCMToken) ? "Inactive" : "Active";
                    return u;
                    }).ToList();

                return Ok(
                    new
                    {
                        users = userDTOs,
                        hasMore = response.Item2,
                        message = response.Item3
                    });
            }
            return Ok(
                    new
                    {
                        users = new List<AUserDTO>(),
                        hasMore = response.Item2,
                        message = response.Item3
                    });
        }

        [HttpGet("users-in-list-userName")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsersInListAsync([FromQuery] ListUserNameDTO request)
        {
            if (!ModelState.IsValid || request.userNames == null || request.userNames.Count == 0)
                return BadRequest("Danh sách userNames không hợp lệ");

            var userTasks = request.userNames.Select(username => _identityService.FindUserByUserNameAsync(username));
            var users = await Task.WhenAll(userTasks);

            var validUsers = users.Where(u => u != null).ToList();

            if (validUsers.Count == 0)
                return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

            var userDTOs = validUsers.Select(user => _mapper.Map<AUserDTO>(user)).ToList();
            return Ok(userDTOs);
        }
        [HttpGet("user")]
        [AllowAnonymous]

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
        public async Task<IActionResult> CheckUser([FromQuery] string? email, [FromQuery] string? userName, [FromQuery] string? userId)
        {
            var userDict = new Dictionary<string, AUser>();

            if (!string.IsNullOrEmpty(email))
            {
                var userByEmail = await _identityService.FindUserByEmailAsync(email);
                if (userByEmail != null) userDict["email"] = userByEmail;
            }

            if (!string.IsNullOrEmpty(userName))
            {
                var userByUserName = await _identityService.FindUserByUserNameAsync(userName);
                if (userByUserName != null) userDict["userName"] = userByUserName;
            }

            if (!string.IsNullOrEmpty(userId))
            {
                var userById = await _identityService.FindUserByIdAsync(userId);
                if (userById != null) userDict["userId"] = userById;
            }

            // Kiểm tra xem có nhiều user khác nhau không
            if (userDict.Values.Select(u => u.Id).Distinct().Count() > 1)
            {
                return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            }

            // Lấy user nếu có, ưu tiên theo thứ tự email -> username -> userId
            var finalUser = userDict.Values.FirstOrDefault();
            var userDTO = finalUser != null ? _mapper.Map<AUserDTO>(finalUser) : null;

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

                if (updateRequest.DeviceId != null)
                    user.DeviceId = updateRequest.DeviceId;


                if (await _identityService.UpdateUserAsync(user))
                    return Ok(LocalValue.Get(KeyStore.UserInfoUpdatedSuccess));
            }
            catch (Exception)
            {
                return BadRequest(LocalValue.Get(KeyStore.GeneralUpdateError));
            }
            return BadRequest(LocalValue.Get(KeyStore.GeneralUpdateError));
        }


        [HttpPut("add-className")]
        public async Task<IActionResult> AddClassName([FromBody] StudentClassName request)
        {
            var tokenInfo = Request.ExtractTokenInfo(_jwtSetting);
            if (tokenInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.ClassName))
            {
                return BadRequest("Request is empty.");
            }

            try
            {

                    AUser? user = await _identityService.FindUserByUserNameAsync(request.UserName);
                if(user == null)
                {
                    return NotFound("No users were updated.");
                }
                    user.ClassName = request.ClassName;

                if (await _identityService.UpdateUserAsync(user))
                {
                    return Ok($"updated successfully.");
                }
                return BadRequest();
                
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating users.");
            }
        }



        [HttpPut("update-user-admin")]
        public async Task<IActionResult> UpdateAdmin([FromBody] UpdateUserAdminDTO updateRequest)
        {
            var tokenInfo = Request.ExtractTokenInfo(_jwtSetting);
            if (tokenInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            if(!await _identityService.CheckRole(tokenInfo.Username,"ADMIN"))
            {
                return BadRequest("Ban khong co quyen admin.");
            }
            var user = await _identityService.FindUserByUserNameAsync(updateRequest.UserName);
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
        [HttpPut("update-avatar-all-develop")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateAllUserAvatar()
        {
            int page = 1;
            int itemUbdated = 0;
            try
            {
                while (true)
                {
                    var filter = Builders<AUser>.Filter.Empty;
                    var users = await _aUserService.GetSlide(page, 10, filter);
                    if (!users.Any())
                    {
                        break;
                    }
                    foreach (var user in users)
                    {
                        if (user == null) continue;
                        user.AvataUrl = KeyStore.GetRandomAvatarDefault(user.Gender);
                        if (await _identityService.UpdateUserAsync(user))
                        {
                            itemUbdated++;
                        }
                    }
                    page++;
                }
                return Ok($"Da cap nhat xong {itemUbdated} nguoi dung.");

            }
            catch (Exception ex) {
                return BadRequest("Update thất bại"+ ex);
            }
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

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordRequestDTO changePasswordDTO)
        {
            // 1. Trích xuất thông tin người dùng từ JWT
            var tokenInfo = Request.ExtractTokenInfo(_jwtSetting);
            if (tokenInfo == null)
            {
                return BadRequest(LocalValue.Get(KeyStore.PasswordUpdateError)); // Không có token hoặc token không hợp lệ
            }

            // 2. Xác định username cần thay đổi mật khẩu
            var targetUsername = changePasswordDTO.UserName;
            var requesterUsername = tokenInfo.Username;

            // Nếu không truyền username => người dùng đổi mật khẩu cho chính mình
            if (string.IsNullOrWhiteSpace(targetUsername))
            {
                targetUsername = requesterUsername;
            }
            else
            {
                // Nếu truyền username khác => chỉ Admin mới được phép đổi mật khẩu cho người khác
                var isAdmin = await _identityService.CheckRole(requesterUsername, "ADMIN");
                if (!isAdmin)
                {
                    return BadRequest(LocalValue.Get(KeyStore.PasswordUpdateError));
                }
            }

            // 3. Gọi service để đổi mật khẩu
            var updated = await _identityService.UpdatePasswordAsync(targetUsername, changePasswordDTO);

            // 4. Trả về kết quả
            return updated
                ? Ok(LocalValue.Get(KeyStore.PasswordUpdated))
                : BadRequest(LocalValue.Get(KeyStore.PasswordUpdateError));
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
