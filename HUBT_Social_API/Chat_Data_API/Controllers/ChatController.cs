using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using MongoDB.Driver;
using HUBT_Social_Core.Settings;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.InitRequest;
using HUBT_Social_Chat_Service.Helper;
using Microsoft.AspNetCore.Identity;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Base;
using AutoMapper;
using Microsoft.Extensions.Options;
using HUBT_Social_Chat_Service.Interfaces;
using System.Collections.Generic;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Linq;
using Chat_Data_API.Src.Hubs;
using Microsoft.AspNetCore.SignalR;
using Amazon.Runtime.Internal;
using Chat_Data_API.Hubs;

namespace Chat_Data_API.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController
        (
            IChatService chatService,
            IUserService userService, 
            IHubContext<ChatHub> hubContext, 
            IUserConnectionManager connectionManager, 
            IMapper mapper,
            IOptions<JwtSetting> options
        ) 
        : DataLayerController(mapper, options)
    {
        private readonly IChatService _chatService  = chatService;
        private readonly IUserService _userService = userService;
        private readonly IHubContext<ChatHub> _hubContext = hubContext;
        private readonly IUserConnectionManager _connectionManager = connectionManager;


        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup(CreateGroupRequest createGroupRequest)
        {
            // Kiểm tra đầu vào
            var validationError = ValidateCreateGroupRequest(createGroupRequest);
            if (validationError != null)
                return BadRequest(validationError);

            // Lấy thông tin người dùng từ token
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo is null)
                return BadRequest("Token is not valid");

            // Tạo danh sách Participant
            var participants = CreateParticipants(createGroupRequest.UserIds, userInfo.UserId.ToString());

            // Tạo ChatRoomModel
            var newChatRoom = CreateChatRoom(createGroupRequest.GroupName, participants.Result);

            // Lưu ChatRoom vào database
            var result = await _chatService.CreateGroupAsync(newChatRoom);
            if (result.Item1)
            {
                foreach (var user in participants.Result)
                {
                    var connectionId = _connectionManager.GetConnectionId(user.UserId);
                    if (connectionId != null)
                    {
                        await _hubContext.Groups.AddToGroupAsync(connectionId, result.Item2);
                    }
                }
                ChatGroupModel? group = await _chatService.GetGroupById(result.Item2);

                if (group != null) 
                {
                    await _hubContext.Clients.Group(result.Item2)
                    .SendAsync("CreateNewGroup",
                        new
                        {
                            group = new GroupLoadingResponse
                            {
                                Id = group.Id,
                                GroupName = group.Name,
                                AvatarUrl = group.AvatarUrl
                            }
                        });
                }
                return Ok(result.Item2);
            }

            return BadRequest(result.Item2);
        }
        // Phương thức kiểm tra đầu vào
            private string? ValidateCreateGroupRequest(CreateGroupRequest request)
            {
                if (string.IsNullOrEmpty(request.GroupName))
                    return LocalValue.Get(KeyStore.GroupNameRequired);
                if (request.UserIds.Count < 2)
                    return "Khong du nguoi";
                return null;
            }
            // Phương thức tạo danh sách Participant
            private async Task<List<Participant>> CreateParticipants(List<string> userIds, string ownerUserId)
            {
                List<AUserDTO> userDTOs = await _userService.GetUser();

                var filteredUsers = userDTOs
                    .Where(user => userIds.Contains(user.Id.ToString()) || user.Id.ToString() == ownerUserId)
                    .ToList();

                return filteredUsers
                    .Select(user => new Participant
                    {
                        UserId = user.Id.ToString(),
                        Role = user.Id.ToString() == ownerUserId ? ParticipantRole.Owner : ParticipantRole.Member,
                        NickName = user.UserName, // Hoặc một giá trị mặc định
                        ProfilePhoto = user.AvataUrl
                    })
                    .ToList();
        }
            // Phương thức tạo ChatRoomModel
            private ChatGroupModel CreateChatRoom(string groupName, List<Participant> participants)
            {
                return new ChatGroupModel
                {
                    Name = groupName,
                    AvatarUrl = LocalValue.Get(KeyStore.DefaultUserImage),
                    Participant = participants,
                    CreatedAt = DateTime.UtcNow
                };
            }

        [HttpDelete("delete-group")]
        public async Task<IActionResult> DeleteGroupAsync([FromQuery] string groupId)
        {
            var result = await _chatService.DeleteGroupAsync(groupId);
            if (result.Item1)
            {
                ChatGroupModel? group = await _chatService.GetGroupById(groupId);
                if (group != null) 
                {
                    foreach (var user in group.Participant)
                    {
                        var connectionId = _connectionManager.GetConnectionId(user.UserId);
                        if (connectionId != null)
                        {
                            await _hubContext.Groups.AddToGroupAsync(connectionId, groupId);
                        }
                    }
                }         
                return Ok(result.Item2);
            }

            return BadRequest(result.Item2);
        }

        [HttpGet("search-groups")]
        public async Task<IActionResult> SearchGroupsAsync([FromQuery] string keyword, [FromQuery] int page = 1, [FromQuery] int limit = 5)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Ok(new List<GroupSearchResponse>());
            List<GroupSearchResponse> chatGroups = await _chatService.SearchGroupsAsync(keyword,page,limit);

            return Ok(chatGroups);
        }

        [HttpGet("get-all-rooms")]
        public async Task<IActionResult> GetAllRoomsAsync([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            List<GroupSearchResponse> chatGroups = await _chatService.GetAllRoomsAsync(page, limit);
            return Ok(chatGroups);
        }

        [HttpGet("get-rooms-of-user")]
        public async Task<IActionResult> GetRoomsOfUserIdAsync([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            // Lấy thông tin người dùng từ token
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo is null)
                return BadRequest(new { message = "Token is not valid" });
    
            List<GroupLoadingResponse> chatGroups = await _chatService.GetRoomsOfUserIdAsync(userInfo.UserId, page, limit);
            return Ok(chatGroups);
        }

    }
}
