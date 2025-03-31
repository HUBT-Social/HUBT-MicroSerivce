using Amazon.Runtime.Internal;
using AutoMapper;
using Chat_Data_API.Src.Hubs;
using HUBT_Social_Base;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.GetRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Helper;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_Chat_Service.Services;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Chat_Data_API.Src.Controllers
{
    [Route("api/room")]
    [ApiController]
    [Authorize]
    public class RoomController
        (
            IRoomUpdateService roomUpdateService,
            IRoomGetService roomGetService,
            IHubContext<ChatHub> hubContext,
            IMapper mapper,
            IOptions<JwtSetting> options,
            IUserConnectionManager connectionManager,
            HUBT_Social_Base.Service.ICloudService clouldService
        ) : DataLayerController(mapper, options)
    {
        private readonly IRoomUpdateService _roomUpdateService = roomUpdateService;
        private readonly IHubContext<ChatHub> _hubContext = hubContext;
        private readonly IRoomGetService _roomGetService = roomGetService;
        private readonly IUserConnectionManager _connectionManager = connectionManager;
        private readonly HUBT_Social_Base.Service.ICloudService _clouldService = clouldService;

        [HttpPut("update-group-name")]
        public async Task<IActionResult> UpdateGroupNameAsync(UpdateGroupNameRequest request)
        {
            TokenInfoDTO? userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            (bool,string) result = await _roomUpdateService.UpdateGroupNameAsync(request.GroupId, request.NewName);

            if (result.Item1)
            {
                await _hubContext.Clients.Group(request.GroupId)
                .SendAsync("UpdateGroupName",
                    new
                    {
                        groupId = request.GroupId,
                        changerId = userInfo.UserId,
                        newGroupName = request.NewName
                    });
                return Ok(result.Item2);
            }

            return BadRequest(result.Item2);

        }

        [HttpPut("update-avatar")]
        public async Task<IActionResult> UpdateAvatarAsync([FromQuery] string groupId, [FromQuery] IFormFile file)
        {
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(new { message = LocalValue.Get(KeyStore.UnAuthorize) });
            if (string.IsNullOrEmpty(groupId) || file == null || file.Length == 0)
                return BadRequest(new { message = "GroupId hoặc file không hợp lệ!" });

            (bool, string) result = await _roomUpdateService.UpdateAvatarGroupAsync(groupId, file);
            if (result.Item1)
            {
                await _hubContext.Clients.Group(groupId)
                .SendAsync("UpdateGroupAvarta",
                    new
                    {
                        groupId,
                        changerId = userInfo.UserId,
                        url = result.Item2
                    });
                return Ok(result.Item2);
            }

            return BadRequest(result.Item2);
        }

        [HttpPut("update-nickname")]
        public async Task<IActionResult> UpdateNickNameAsync(UpdateNickNameRequest request)
        {
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            (bool, string) result = await _roomUpdateService.UpdateNickNameAsync(request.GroupId, request.UserId, request.NewNickName);
            if (result.Item1)
            {
                await _hubContext.Clients.Group(request.GroupId)
                .SendAsync("UpdateNickName",
                    new
                    {
                        groupId = request.GroupId,
                        changerId = userInfo.UserId,
                        changedId = request.UserId,
                        newNickName = request.NewNickName
                    });
                return Ok(result.Item2);
            }

            return BadRequest(result.Item2);

        }

        [HttpPut("update-participant-role")]
        public async Task<IActionResult> UpdateParticipantRoleAsync(UpdateParticipantRoleRequest request)
        {
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            (bool, string) result = await _roomUpdateService.UpdateParticipantRoleAsync(request.groupId, request.userId, request.participantRole);
            if (result.Item1)
            {
                await _hubContext.Clients.Group(request.groupId)
                .SendAsync("UpdateParticipantRole",
                    new
                    {
                        request.groupId,
                        changerId = userInfo.UserId,
                        changedId = request.userId,
                        newRole = request.participantRole
                    });
                return Ok(result.Item2);
            }

            return BadRequest(result.Item2);
        }



        [HttpPost("join-room")]
        public async Task<IActionResult> JoinRoomAsync(AddMemberRequestData request)
        {
            if(request.Participant == null)
            {
                return BadRequest("Participant is null");
            }
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            (bool, string) result = await _roomUpdateService.JoinRoomAsync(request.GroupId, request.Participant);
            if (result.Item1)
            {
                var connectionId = _connectionManager.GetConnectionId(request.Participant.UserId);
                if (connectionId != null)
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, request.GroupId);
                }
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(request.GroupId)
                .SendAsync("UserJoinedGroup",
                    new
                    {
                        groupId = request.GroupId,
                        AdderId = userInfo.UserId,
                        AddedId = request.Participant
                    });
                return Ok(result.Item2);
            }

            return BadRequest(result.Item2);
        }

        [HttpPost("kick-member")]
        public async Task<IActionResult> KickMemberAsync(RemoveMemberRequest request)
        {
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            (bool, string) result = await _roomUpdateService.KickMemberAsync(request);
            if (result.Item1)
            {
                var connectionId = _connectionManager.GetConnectionId(request.KickedId);
                if (connectionId != null)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, request.GroupId);
                }
                await _hubContext.Clients.Group(request.GroupId)
                .SendAsync("KickMember",
                    new
                    {
                        groupId = request.GroupId,
                        KickerId = userInfo.UserId,
                        request.KickedId
                    });
                return Ok(result.Item2);
            }

            return BadRequest(result.Item2);
        }

        [HttpPost("leave-room")]
        public async Task<IActionResult> LeaveRoomAsync(LeaveRoomRequest request)
        {
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            var result = await _roomUpdateService.LeaveRoomAsync(request.GroupId, userInfo.UserId);
            if (result.Item1)
            {
                var connectionId = _connectionManager.GetConnectionId(userInfo.UserId);
                if (connectionId != null)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, request.GroupId);
                }
                await _hubContext.Clients.Group(request.GroupId)
                    .SendAsync("LeaveRoom",
                        new
                        {
                            groupId = request.GroupId,
                            userId = userInfo.UserId
                        }
                    );
                return Ok(result.Item2);
            }

            return BadRequest(result.Item2);
        }

        [HttpGet("message-history")]
        public async Task<IActionResult> GetMessageHistoryAsync([FromQuery] GetHistoryRequest request)
        {
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            List<MessageModel> messageModels = await _roomGetService.GetMessageHistoryAsync(request);

            MessageResponse<List<MessageDTO>> response = new MessageResponse<List<MessageDTO>>
            {
                groupId = request.ChatRoomId,
                message = _mapper.Map<List<MessageDTO>>(messageModels)
            };

            return Ok(response);
        }

        [HttpGet("get-members")]
        public async Task<IActionResult> GetRoomUserAsync([FromQuery] string groupId)
        {
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            (List<ChatUserResponse>, ChatGroupModel) response = await _roomGetService.GetRoomUserAsync(groupId);
            if(!response.Item1.Any())
            {                
                return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            }
            return Ok
            (
                new GetMemberGroup
                {
                    title  = response.Item2.Name,
                    avatarUrl = response.Item2.AvatarUrl,
                    caller = userInfo.UserId,
                    response = response.Item1
                }
            );
        }


    }
}
