using Chat_API.Src.Interfaces;
using HUBT_Social_Chat_Resources.Dtos.Request.GetRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.InitRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("get-message-history")]
        public async Task<IActionResult> GetMessageHistory(GetHistoryRequest request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            var messages = await _roomService.GetMessageHistoryAsync(request, token);
            return Ok(messages);
        }

        [HttpGet("get-room-users")]
        public async Task<IActionResult> GetRoomUser(GetMemberInGroupRequest request)
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            var users = await _roomService.GetRoomUserAsync(request, token);
            return Ok(users);
        }

    }
}
