using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.GetRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Service.Interfaces
{

        public interface IRoomUpdateService
        {
            Task<(bool, string)> UpdateGroupNameAsync(string groupId, string newName);
            Task<(bool, string)> UpdateAvatarGroupAsync(string groupId, IFormFile file);
            Task<(bool, string)> UpdateNickNameAsync(string groupId, string changedId, string newNickName);
            Task<(bool, string)> UpdateParticipantRoleAsync(string groupId, string changedId, ParticipantRole newParticipantRole);
            Task<(bool, string)> JoinRoomAsync(string groupId, Participant? added);
            Task<(bool, string)> KickMemberAsync(RemoveMemberRequest request);
            Task<(bool, string)> LeaveRoomAsync(string groupId, string userId);
        }
        public interface IRoomGetService
        {
            Task<List<MessageModel>> GetMessageHistoryAsync(GetHistoryRequest request);
            Task<List<ChatUserResponse>> GetRoomUserAsync(string groupId);
        }
        
}
