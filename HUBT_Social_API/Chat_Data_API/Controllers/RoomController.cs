using Amazon.Runtime.Internal;
using AutoMapper;
using Chat_Data_API.Hubs;
using Chat_Data_API.Src.Hubs;
using HUBT_Social_Base;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.GetRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Helper;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Chat_Data_API.Controllers
{
    [Route("api/room")]
    [ApiController]
    [Authorize]
    public class RoomController
        (
            IRoomUpdateService roomUpdateService, 
            IRoomGetService roomGetService,
            IUserService userService,
            IHubContext<ChatHub> hubContext,
            IMapper mapper, 
            IOptions<JwtSetting> options,
            IUserConnectionManager connectionManager
        ) : DataLayerController(mapper, options)
    {
        private readonly IRoomUpdateService _roomUpdateService = roomUpdateService;
        private readonly IHubContext<ChatHub> _hubContext = hubContext;
        private readonly IRoomGetService _roomGetService = roomGetService;
        private readonly IUserService _userService = userService;
        private readonly IUserConnectionManager _connectionManager = connectionManager;


        [HttpPut("update-group-name")]
        public async Task<IActionResult> UpdateGroupNameAsync(UpdateGroupNameRequest request)
        {
            TokenInfoDTO? userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            var result = await _roomUpdateService.UpdateGroupNameAsync(request.GroupId, request.NewName);

            if(result.Item1)
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
            
            return  BadRequest(result.Item2);
            
        }

        //[HttpPut("update-avatar")]
        //public async Task<IActionResult> UpdateAvatarAsync([FromQuery] string groupId, [FromQuery] string newUrl)
        //{
        //    var userInfo = Request.GetUserInfoFromRequest();
        //    if (userInfo == null)
        //        return Unauthorized(new { message = LocalValue.Get(KeyStore.UnAuthorize) });

            

        //}

        [HttpPut("update-nickname")]
        public async Task<IActionResult> UpdateNickNameAsync(UpdateNickNameRequest request)
        {
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            var result = await _roomUpdateService.UpdateNickNameAsync(request.GroupId, request.UserId, request.NewNickName);
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
            var result = await _roomUpdateService.UpdateParticipantRoleAsync(request.groupId, request.userId, request.participantRole);
            if (result.Item1)
            {
                await _hubContext.Clients.Group(request.groupId)
                .SendAsync("UpdateParticipantRole",
                    new
                    {
                        groupId = request.groupId,
                        changerId = userInfo.UserId,
                        changedId = request.userId,
                        newRole = request.participantRole
                    });
                return Ok(result.Item2);
            }

            return BadRequest(result.Item2);
        }

        

        [HttpPost("join-room")]
        public async Task<IActionResult> JoinRoomAsync(AddMemberRequest request)
        {
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            AUserDTO? userDTO = await _userService.GetUserById(request.AddedId);

            Participant? Added = (!string.IsNullOrEmpty(request.AddedId) && userDTO != null) ? new Participant
            {
                UserId = request.AddedId,
                Role = ParticipantRole.Member,
                NickName = userDTO.FullName,
                ProfilePhoto = userDTO.AvataUrl
            } : null;
            
            var result = await _roomUpdateService.JoinRoomAsync(request.GroupId, Added);
            if (result.Item1)
            {
                var connectionId = _connectionManager.GetConnectionId(request.AddedId);
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
                        AddedId = request.AddedId
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
            var result = await _roomUpdateService.KickMemberAsync(request);
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
                        KickedId = request.KickedId
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
        public async Task<IActionResult> GetMessageHistoryAsync([FromQuery] GetHistoryRequest request) =>
             Ok(await _roomGetService.GetMessageHistoryAsync(request));


        [HttpGet("get-members")]
        public async Task<IActionResult> GetRoomUserAsync([FromQuery] GetMemberInGroupRequest request) =>
            Ok(await _roomGetService.GetRoomUserAsync(request.groupId));
        
    }
}
