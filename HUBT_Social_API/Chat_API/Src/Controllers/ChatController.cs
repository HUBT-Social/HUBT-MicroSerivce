using CloudinaryDotNet.Actions;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBT_Social_API.Src.Core.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

namespace HUBT_Social_API.Features.Chat.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ITokenService _tokenService;
    private readonly IRoomService _roomService;
    private readonly UserManager<AUser> _userManager;

    public ChatController(IChatService chatService,ITokenService tokenService,IRoomService roomService,UserManager<AUser> userManager)
    {
        _chatService = chatService;
        _tokenService = tokenService;
        _roomService = roomService;
        _userManager = userManager;
    }

    /// <summary>
    ///     Tạo nhóm chat mới.
    /// </summary>
    [HttpPost("create-group")]
    public async Task<IActionResult> CreateGroup(CreateGroupRequest createGroupRequest)
    {
        // Kiểm tra đầu vào
        var validationError = ValidateCreateGroupRequest(createGroupRequest);
        if (validationError != null)
            return BadRequest(new { message = validationError });

        // Lấy thông tin người dùng từ token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (!userResponse.Success)
            return BadRequest(new { message = "Token is not valid" });

        // Tạo danh sách Participant
        var participants = CreateParticipants(createGroupRequest.UserIds, userResponse.User.Id.ToString());

        // Tạo ChatRoomModel
        var newChatRoom = CreateChatRoom(createGroupRequest.GroupName, participants.Result);

        // Lưu ChatRoom vào database
        var groupId = await _chatService.CreateGroupAsync(newChatRoom);
        return groupId != null
            ? Ok(new { message = groupId })
            : BadRequest(new { message = LocalValue.Get(KeyStore.FailedToCreateGroup) });
    }
            // Phương thức kiểm tra đầu vào
            private string? ValidateCreateGroupRequest(CreateGroupRequest request)
            {
                if (string.IsNullOrEmpty(request.GroupName))
                    return LocalValue.Get(KeyStore.GroupNameRequired);
                if (request.UserIds.Count < 2)
                    return LocalValue.Get(KeyStore.NotEnoughMembers);
                return null;
            }
            // Phương thức tạo danh sách Participant
            private async Task<List<Participant>> CreateParticipants(IEnumerable<string> userIds, string ownerUserId)
            {
                var participants = await Task.WhenAll(userIds.Select(userId => Participant.CreateAsync(_userManager, userId, null)));

                List<Participant> participantList = participants.ToList();

                Participant owner = await Participant.CreateAsync(_userManager, ownerUserId, ParticipantRole.Owner);
                participantList.Add(owner);

                return participantList;
            }
            // Phương thức tạo ChatRoomModel
            private ChatRoomModel CreateChatRoom(string groupName, List<Participant> participants)
            {
                return new ChatRoomModel
                {
                    Name = groupName,
                    AvatarUrl = LocalValue.Get(KeyStore.DefaultUserImage),
                    Participant = participants,
                    CreatedAt = DateTime.UtcNow
                };
            }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpDelete("delete-group")]
    public async Task<IActionResult> DeleteGroupAsync(DeleteGroupRequest request)
    {
        if (string.IsNullOrEmpty(request.GroupId))
            return BadRequest(new { message = "Group ID is required." });

        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (!userResponse.Success)
            return BadRequest("Token is not valid");

        if(await RoomChatHelper.GetRoleAsync(request.GroupId,userResponse.User.UserName) != ParticipantRole.Admin)
        {
            return BadRequest("You cant delete this group because you are not owner.");
        }

        var result = await _chatService.DeleteGroupAsync(request.GroupId);
        if (result)
            return Ok("Group deleted successfully.");

        return NotFound("Group not found or could not be deleted.");
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet("search-group")]
    public async Task<IActionResult> SearchGroupsAsync([FromQuery] string keyword, [FromQuery] int page=1, [FromQuery] int limit=5)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest("Keyword is required.");

        var groups = await _chatService.SearchGroupsAsync(keyword,page,limit);
        if (groups.Any())
            return Ok(groups);

        return Ok(new List<RoomSearchReponse>());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet("develop/get-all-group")]
    public async Task<IActionResult> GetAllRoomsAsync( [FromQuery] int page=1, [FromQuery] int limit=10)
    {
        var rooms = await _chatService.GetAllRoomsAsync(page,limit);
        return Ok(rooms);
    }
    
    /// <summary>
    /// DeveloperDeveloper
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("develop/load-rooms-by-username")]
    public async Task<IActionResult> GetRoomsByUserNameAsync([FromQuery] string userName,[FromQuery]  int page=1, [FromQuery] int limit=20)
    {
        if (string.IsNullOrEmpty(userName))
            return BadRequest("Username is required.");
        string? userId = await UserHelper.GetUserIdByUserName(_userManager,userName);
        if(string.IsNullOrEmpty(userId))
            return BadRequest("UserName is not exits");
        
        var rooms = await _chatService.GetRoomsOfUserIdAsync(userName,page,limit);
        if (rooms.Any())
            return Ok(rooms);

        return NotFound("No rooms found for the user.");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet("load-rooms")]
    public async Task<IActionResult> GetRoomsByTokenTokenAsync([FromQuery]  int page=1, [FromQuery] int limit=10)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        Console.WriteLine("read access token success");
        if (userResponse.Success == false)
        {
            return BadRequest("Token is not valid");
        }
        List<RoomLoadingRespone> rooms = await _chatService.GetRoomsOfUserIdAsync(userResponse.User.Id.ToString(),page,limit);
        return Ok(rooms);
    }

   


}