using HUBT_Social_Base;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.GetRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Chat_API.Src.Interfaces
{
    public interface IRoomService : IBaseService
    {
        Task<(bool, string?)> UpdateGroupNameAsync(UpdateGroupNameRequest request, string token);
        //Task<bool> UpdateAvatarAsync(string groupId, string userId, string newUrl, string token);
        Task<(bool, string?)> UpdateNickNameAsync(UpdateNickNameRequest request, string token);
        Task<(bool, string?)> UpdateParticipantRoleAsync(UpdateParticipantRoleRequest request, string token);
        Task<(bool, string?)> JoinRoomAsync(AddMemberRequestData request, string token);
        Task<(bool, string?)> KickMemberAsync(RemoveMemberRequest request, string token);
        Task<(bool, string?)> LeaveRoomAsync(LeaveRoomRequest request, string token);
        Task<MessageResponse<List<MessageDTO>>?> GetMessageHistoryAsync(GetHistoryRequest getItemsHistoryRequest, string token);
        Task<GetMemberGroup> GetRoomUserAsync([FromQuery] GetMemberInGroupRequest request, string token);
    }
}
