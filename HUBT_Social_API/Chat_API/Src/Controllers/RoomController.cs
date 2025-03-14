using Chat_API.Src.Interfaces;
using HUBT_Social_Chat_Resources.Dtos.Request.GetRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.InitRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Chat_API.Src.Controllers
{
    [Route("api/chat/room")]
    [ApiController]
    public class RoomController(IRoomService roomService) : ControllerBase
    {
        private readonly IRoomService _roomService = roomService;
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

            var (success, message) = await _roomService.JoinRoomAsync(request, token);
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

        [HttpGet("get-member")]
        public async Task<IActionResult> GetRoomUser([FromQuery] string groupId)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            GetMemberInGroupRequest request = new GetMemberInGroupRequest()
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

            var currentUser = users.response.FirstOrDefault(u => u.id == users.caller);

            if (currentUser == null)
            {
                return Unauthorized("Người dùng hiện tại không có trong phòng chat.");
            }

            return Ok(new
            {
                GroupId = groupId,
                CurrentUser = currentUser,
                OtherUsers = users.response
                    .Where(u => u.id != users.caller)
                    .ToList()
            });

        }

    }
}
