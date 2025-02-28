using System.Security.Claims;
using HUBT_Social_API.Core.Service.Upload;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBT_Social_API.Src.Core.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization;


namespace HUBT_Social_API.Features.Chat.Controllers;

[ApiController]
[Route("api/chat/room")]
public class RoomController : ControllerBase
{

    private readonly ITokenService _tokenService;
    private readonly IRoomService _roomService;
    private readonly UserManager<AUser> _userManager;

    public RoomController(ITokenService tokenService,IRoomService roomService,UserManager<AUser> userManager)
    {
        _tokenService = tokenService;
        _roomService = roomService;
        _userManager = userManager;
    }
    


   [HttpGet("get-history-chat")]
public async Task<IActionResult> GetHistoryChat([FromQuery] GetHistoryRequest getHistoryRequest)
{
    if (getHistoryRequest == null)
    {
        return BadRequest("Request body cannot be null");
    }

    if (string.IsNullOrWhiteSpace(getHistoryRequest.ChatRoomId))
    {
        return BadRequest("ChatRoomId cannot be null or empty");
    }

    // Kiểm tra null cho Time
    if (getHistoryRequest.Time == null)
    {
        getHistoryRequest.Time = DateTime.Now;
    }

    // Lấy danh sách tin nhắn
    List<MessageModel> messages = await _roomService.GetMessageHistoryAsync(getHistoryRequest);

    return Ok(messages);

}
    
    [Authorize]
    [HttpGet("get-room-user")]
    public async Task<IActionResult> GetRoomUser([FromQuery] string groupId)
    {
        if (string.IsNullOrEmpty(groupId))
        {
            return BadRequest(new { Message = "GroupId không được để trống." });
        }

        var res = await _roomService.GetRoomUserAsync(groupId);

        if (res.Count == 0)
        {
            return NotFound(new { Message = $"Không tìm thấy phòng chat với groupId: {groupId}." });
        }
        // Giả định UserId hiện tại được lấy từ token (hoặc cấu hình)
        var currentUserId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized(new { Message = "Không xác định được người dùng hiện tại." });
        }

        // Lọc currentUser và otherUsers
        var currentUser = res.FirstOrDefault(p => p.id == currentUserId);
        if (currentUser == null)
        {
            return Unauthorized(new { Message = "Người dùng hiện tại không có trong phòng chat." });
        }

        var otherUsers = res.Where(p => p.id != currentUserId).ToList();

        // Ánh xạ sang ChatUserDTO
        var currentUserDto = new ChatUserResponse
        {
            id = currentUser.id,
            name = currentUser.name ?? "Unknown",
            profilePhoto = currentUser.profilePhoto
        };

        var otherUsersDto = otherUsers.Select(p => new ChatUserResponse
        {
            id = p.id,
            name = p.name ?? "Unknown",
            profilePhoto = p.profilePhoto
        }).ToList();

        // Trả về kết quả
        return Ok(new
        {
            GroupId = groupId,
            CurrentUser = currentUserDto,
            OtherUsers = otherUsersDto
        });
    }
 
 // API to update group name
    [HttpPut("update-group-name")]
    public async Task<IActionResult> UpdateGroupName(UpdateGroupNameRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.NewName))
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }

        var result = await _roomService.UpdateGroupNameAsync(request.Id,userResponse.User.Id.ToString(),request.NewName);
        if (result)
        {
            return Ok("Group name updated successfully.");
        }
        return StatusCode(500, "Failed to update group name.");
    }

    // API to update avatar
    [HttpPut("update-group-avatar")]
    public async Task<IActionResult> UpdateAvatar(string GroupId,IFormFile file)
    {
        if (string.IsNullOrEmpty(GroupId) || file == null)
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }
        FileUploadResult? newUrl = await UploadToStoreS3.CloudinaryService.UploadToStorageAsync(file);

        var result = await _roomService.UpdateAvatarAsync(GroupId,userResponse.User.Id.ToString(),newUrl.Url);
        if (result)
        {
            return Ok("Avatar updated successfully.");
        }
        return StatusCode(500, "Failed to update avatar.");
    }

    [HttpPut("update-nickname")]
    public async Task<IActionResult> UpdateNickNameAsync(UpdateNickNameRequest request)
    {
        if (request == null 
            || string.IsNullOrEmpty(request.GroupId) 
            || string.IsNullOrEmpty(request.UserId)
            || string.IsNullOrEmpty(request.NewNickName)
            )
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }

        var result = await _roomService.UpdateNickNameAsync(request.GroupId,userResponse.User.Id.ToString(), request.UserId, request.NewNickName);
        if (result)
            return Ok("Nickname updated successfully.");
        return BadRequest("Failed to update nickname.");
    }
    // API to add a member to the group
    [HttpPut("add-member")]
    public async Task<IActionResult> JoinRoomAsync(AddMemberInputRequest request)
    {
        if (request == null 
        || string.IsNullOrEmpty(request.GroupId) 
        || string.IsNullOrEmpty(request.AddedId)
        )
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }
        Participant participant = await Participant.CreateAsync(_userManager, request.AddedId, null);
        var result = await _roomService.JoinRoomAsync(new AddMemberRequest{GroupId = request.GroupId,Added=participant},userResponse.User.Id.ToString());
        if (result)
        {   
            return Ok("Member added successfully.");
        }
        return StatusCode(500, "Failed to add member.");
    }

    // API to remove a member from the group
    [HttpPost("kick-member")]
    public async Task<IActionResult> KickMemberAsync(RemoveMemberRequest request)
    {
        // Kiểm tra đầu vào
        if (request == null 
        || string.IsNullOrEmpty(request.GroupId) 
        || string.IsNullOrEmpty(request.KickedId))
        {
            return BadRequest("Invalid request.");
        }

        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }

        // Lấy vai trò của người thực hiện hành động (kicker) và người bị kick (kicked)
        ParticipantRole? kickerRole = await RoomChatHelper.GetRoleAsync(request.GroupId, userResponse.User.Id.ToString());
        ParticipantRole? kickedRole = await RoomChatHelper.GetRoleAsync(request.GroupId, request.KickedId);

        // Kiểm tra quyền hạn của kicker
        if (kickerRole == null || kickedRole == null)
        {
            return BadRequest("Kicker or kicked user does not exist in the group.");
        }

        if (kickerRole <= kickedRole)
        {
            return BadRequest("You do not have sufficient permissions to remove this member.");
        }

        // Thực hiện hành động xóa thành viên
        bool result = await _roomService.KickMemberAsync(request,userResponse.User.UserName);
        if (result)
        {
            return Ok("Member removed successfully.");
        }

        // Xử lý khi xóa thất bại
        return StatusCode(500, "Failed to remove member.");
    }
    [HttpPost("leave-group")]
    public async Task<IActionResult> LeaveRoomAsync(LeaveRoomRequest request)
    {
        // Kiểm tra đầu vào
        if (request == null 
        || string.IsNullOrEmpty(request.GroupId))
        {
            return BadRequest("Invalid request.");
        }

        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }

        // Lấy vai trò của người thực hiện hành động (kicker) và người bị kick (kicked)
        ParticipantRole? leaver = await RoomChatHelper.GetRoleAsync(request.GroupId, userResponse.User.UserName);
        

        // Kiểm tra quyền hạn của kicker
        if (leaver == null)
        {
            return BadRequest("Kicker or kicked user does not exist in the group.");
        }

        if (leaver == ParticipantRole.Owner)
        {
            return BadRequest("Let give your role to other member and try to exist after you have given role to someone.");
        }

        // Thực hiện hành động xóa thành viên
        bool result = await _roomService.LeaveRoomAsync(request.GroupId,userResponse.User.UserName);
        if (result)
        {
            return Ok("Member removed successfully.");
        }

        // Xử lý khi xóa thất bại
        return StatusCode(500, "Failed to remove member.");
    }
    
    [HttpPut("update-action-status")]
    public async Task<ActionResult> UpdateActionStatusAsync(UpdateStatusMessageRequest unsendMessageRequest)
    {
        if(string.IsNullOrEmpty(unsendMessageRequest.ChatRoomId) 
        || string.IsNullOrEmpty(unsendMessageRequest.MessageId))
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }
        MessageModel? senderOfItem = await RoomChatHelper.GetInfoMessageAsync(unsendMessageRequest.ChatRoomId,unsendMessageRequest.MessageId);
        if(senderOfItem == null)
        {
            return BadRequest("Invalid value request.");
        }
        if(senderOfItem.sentBy != userResponse.User.UserName)
        {
            return BadRequest("You are not owner of this message");
        }
        var updated = await _roomService.UpdateActionStatusAsync(unsendMessageRequest.ChatRoomId,unsendMessageRequest.MessageId,unsendMessageRequest.messageActionStatus);
        return updated ? Ok("Updated") : BadRequest("Err to Update.");
    }
   

}




