using HUBT_Social_API.Features.Chat.Controllers;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using static HUBT_Social_API.Features.Chat.Services.RoomService;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IRoomService
{
    //Update methods
    // Update methods
    /// <summary>
    /// Cập nhật tên nhóm cho phòng chat.
    /// </summary>
    /// <param name="id">ID của phòng chat cần cập nhật.</param>
    /// <param name="newName">Tên nhóm mới.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    Task<bool> UpdateGroupNameAsync(string id, string userName, string newName);

    /// <summary>
    /// Cập nhật URL avatar cho phòng chat.
    /// </summary>
    /// <param name="id">ID của phòng chat cần cập nhật.</param>
    /// <param name="newUrl">URL mới cho avatar.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    Task<bool> UpdateAvatarAsync(string id, string userName, string newUrl);

    /// <summary>
    /// Cập nhật biệt danh (nickname) của một thành viên trong phòng chat.
    /// </summary>
    /// <param name="roomId">ID của phòng chat.</param>
    /// <param name="userName">Tên người dùng cần cập nhật biệt danh.</param>
    /// <param name="newNickName">Biệt danh mới.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    Task<bool> UpdateNickNameAsync(string roomId,string changerName ,string userName, string newNickName);
    /// <summary>
    /// Cập nhật vai trò của một thành viên trong phòng chat.
    /// </summary>
    /// <param name="roomId">ID của phòng chat.</param>
    /// <param name="userName">Tên người dùng cần cập nhật vai trò.</param>
    /// <param name="newParticipantRole">Vai trò mới cần gán.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    Task<bool> UpdateParticipantRoleAsync(string roomId, string changerId ,string changedId, ParticipantRole newParticipantRole);
    Task<bool> UpdateActionStatusAsync(string roomId, string chatItemId,MessageActionStatus newActionStatus);
    /// <summary>
    /// Thêm một thành viên mới vào phòng chat.
    /// </summary>
    /// <param name="request">Yêu cầu thêm thành viên.</param>
    /// <returns>Trả về true nếu thêm thành công, ngược lại false.</returns>
    Task<bool> JoinRoomAsync(AddMemberRequest request,string AdderName);
    /// <summary>
    /// Xóa một thành viên khỏi phòng chat.
    /// </summary>
    /// <param name="request">Yêu cầu xóa thành viên.</param>
    /// <returns>Trả về true nếu xóa thành công, ngược lại false.</returns>
    Task<bool> KickMemberAsync(RemoveMemberRequest request,string KickerName);
    Task<bool> LeaveRoomAsync(string groupId,string userName);



    //Get
    Task<List<MessageModel>> GetMessageHistoryAsync(GetHistoryRequest getItemsHistoryRequest);
    Task<List<ChatUserResponse>> GetRoomUserAsync(string groupId);
} 