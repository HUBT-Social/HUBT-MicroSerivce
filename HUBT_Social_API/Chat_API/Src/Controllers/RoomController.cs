using Amazon.Runtime.Internal;
using Chat_API.Src.Interfaces;
using Hangfire.Mongo.Dto;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.GetRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.InitRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Chat_API.Src.Controllers
{
    [Route("api/chat/room")]
    [ApiController]
    public class RoomController(IRoomService roomService, IUserService userService) : ControllerBase
    {
        private readonly IRoomService _roomService = roomService;
        public readonly IUserService _userService = userService;
        private string? ReadTokenFromHeader() => Request.Headers.ExtractBearerToken();

        [HttpPut("update-group-name")]
        public async Task<IActionResult> UpdateGroupName(UpdateGroupNameRequest request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            var (success, message) = await _roomService.UpdateGroupNameAsync(request, token);
            return success ? Ok(message) : BadRequest(message);
        }

        [HttpPut("update-nickname")]
        public async Task<IActionResult> UpdateNickName(UpdateNickNameRequest request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            var (success, message) = await _roomService.UpdateNickNameAsync(request, token);
            return success ? Ok(message) : BadRequest(message);
        }

        [HttpPut("update-participant-role")]
        public async Task<IActionResult> UpdateParticipantRole(UpdateParticipantRoleRequest request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            var (success, message) = await _roomService.UpdateParticipantRoleAsync(request, token);
            return success ? Ok(message) : BadRequest(message);
        }

        [HttpPost("join-room")]
        public async Task<IActionResult> JoinRoom(AddMemberRequest request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            if(request == null || string.IsNullOrEmpty(request.GroupId) || string.IsNullOrEmpty(request.GroupId))
            {
                return BadRequest();
            }
            AUserDTO? user = await _userService.GetUserByUserName(request.Added, token);
            if (user == null) { return BadRequest(); }
            Participant participant = new()
            {
                UserName = user.UserName,
                Role = ParticipantRole.Member,
                NickName = user.LastName + " " + user.FirstName, // Hoặc một giá trị mặc định
                ProfilePhoto = user.AvataUrl
            };
            AddMemberRequestData addMemberRequestData = new()
            {
                GroupId = request.GroupId,
                Participant = participant
            };
            var (success, message) = await _roomService.JoinRoomAsync(addMemberRequestData, token);
            return success ? Ok(message) : BadRequest(message);
        }

        [HttpPost("kick-member")]
        public async Task<IActionResult> KickMember(RemoveMemberRequest request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            var (success, message) = await _roomService.KickMemberAsync(request, token);
            return success ? Ok(message) : BadRequest(message);
        }

        [HttpPost("leave-room")]
        public async Task<IActionResult> LeaveRoom(LeaveRoomRequest request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            var (success, message) = await _roomService.LeaveRoomAsync(request, token);
            return success ? Ok(message) : BadRequest(message);
        }

        [HttpGet("get-history")]
        public async Task<IActionResult> GetMessageHistory([FromQuery] GetHistoryRequest request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            var messages = await _roomService.GetMessageHistoryAsync(request, token);
            return messages != null ? Ok(messages.message) : BadRequest();
        }

        [HttpGet("get-room-user")]
        public async Task<IActionResult> GetUser([FromQuery] GetMemberInGroupRequest request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            var users = await _roomService.GetRoomUserAsync(request, token);
            return Ok(users.response);
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetRoomUser([FromQuery] string groupId)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            GetMemberInGroupRequest request = new()
            {
                groupId = groupId
            };
            GetMemberGroup users = await _roomService.GetRoomUserAsync(request, token);

            if (users == null || users.response == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(users.caller))
            {
                return Unauthorized();
            }

            ChatUserResponse?  currentUser = users.response.FirstOrDefault(u => u.userName == users.caller);

            if (currentUser == null)
            {
                return Unauthorized("Người dùng hiện tại không có trong phòng chat.");
            }

            return Ok(new
            {
                Title = users.title,
                AvatarUrl = users.avatarUrl,
                CurrentUser = currentUser,
                OtherUsers = users.response
                    .Where(u => u.userName != users.caller)
                    .ToList()
            });

        }

        [HttpGet("get-user-test")]
        public async Task<IActionResult> GetUsersTest([FromQuery] List<string> request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            List<AUserDTO>? userDTOs = await _userService.GetUsersByUserNames(request, token);
            if (userDTOs == null  || userDTOs.Count == 0) { return BadRequest(); }

            List<Participant> participants = [];

            foreach (var user in userDTOs)
            {
                Participant participant = new()
                {
                    UserName = user.UserName,
                    Role = ParticipantRole.Member,
                    NickName = user.LastName + user.FirstName, // Hoặc một giá trị mặc định
                    ProfilePhoto = user.AvataUrl
                };
                participants.Add(participant);
            }
           
            return Ok(participants);
            
        }
    }
}
