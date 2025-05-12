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
using Microsoft.AspNetCore.SignalR;
using Amazon.Runtime.Internal;
using Chat_Data_API.Src.Hubs;
using Chat_Data_API.Src.Service;

namespace Chat_Data_API.Src.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController
        (
            IChatService chatService,
            IHubContext<ChatHub> hubContext,
            IUserConnectionManager connectionManager,
            IMapper mapper,
            IOptions<JwtSetting> options
        )
        : DataLayerController(mapper, options)
    {
        private readonly IChatService _chatService = chatService;

        private readonly IHubContext<ChatHub> _hubContext = hubContext;
        private readonly IUserConnectionManager _connectionManager = connectionManager;



        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup(CreateGroupRequestData createGroupRequest)
        {
            // Kiểm tra đầu vào
            var validationError = ValidateCreateGroupRequest(createGroupRequest);
            if (validationError != null)
                return BadRequest(validationError);

            // Lấy thông tin người dùng từ token
            var userInfo = Request.GetUserInfoFromRequest();
            if (userInfo is null)
                return BadRequest("Token is not valid");

            // Tạo ChatRoomModel
            var newChatRoom = CreateChatRoom(createGroupRequest.GroupName, createGroupRequest.Participants);

            // Lưu ChatRoom vào database
            // Gửi yêu cầu tạo một group mới, kết quả nhận về sẽ gômf một tuple ( status, message) 
            var result = await _chatService.CreateGroupAsync(newChatRoom);
            if (result.Item1)
            {
                foreach (var user in createGroupRequest.Participants)
                {
                    var connectionId = _connectionManager.GetConnectionId(user.UserName);
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
        private string? ValidateCreateGroupRequest(CreateGroupRequestData request)
        {
            if (string.IsNullOrEmpty(request.GroupName))
                return LocalValue.Get(KeyStore.GroupNameRequired);
            if (request.Participants.Count < 2)
                return "Khong du nguoi";
            return null;
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
                        var connectionId = _connectionManager.GetConnectionId(user.UserName);
                        if (connectionId != null)
                        {
                            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupId);
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
            List<GroupSearchResponse> chatGroups = await _chatService.SearchGroupsAsync(keyword, page, limit);

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

            List<GroupLoadingResponse> chatGroups = await _chatService.GetRoomsOfUserAsync(userInfo.Username, page, limit);
            return Ok(chatGroups);
        }

    }
}
