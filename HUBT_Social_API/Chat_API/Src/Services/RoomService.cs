
using Chat_API.Src.Interfaces;
using HUBT_Social_Base.Service;
using HUBT_Social_Base;
using HUBT_Social_Core.Settings.@enum;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest;
using HUBT_Social_Chat_Resources.Dtos.Request.GetRequest;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Resources.Dtos.Response;
using Microsoft.AspNetCore.Mvc;

namespace Chat_API.Src.Services
{
    public class RoomService(IHttpService httpService, string basePath) : BaseService(httpService,basePath), IRoomService
    {


        public async Task<(bool,string?)> UpdateGroupNameAsync(UpdateGroupNameRequest request, string token)
        {
            var response = await SendRequestAsync($"api/room/update-group-name", ApiType.PUT, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
        }

        //public async Task<bool> UpdateAvatarAsync(string groupId, string userId, string newUrl, string token);
        //{
        //    var response = await SendRequestAsync($"api/room/update-avatar?groupId={groupId}&newUrl={newUrl}", ApiType.PUT);
        //    return response.StatusCode == System.Net.HttpStatusCode.OK;
        //}

        public async Task<(bool, string?)> UpdateNickNameAsync(UpdateNickNameRequest request, string token)
    {
            var response = await SendRequestAsync($"api/room/update-nickname", ApiType.PUT, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
        }

        public async Task<(bool, string?)> UpdateParticipantRoleAsync(UpdateParticipantRoleRequest request, string token)
        { 
            var response = await SendRequestAsync($"api/room/update-participant-role", ApiType.PUT, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
        }


        public async Task<(bool, string?)> JoinRoomAsync(AddMemberRequest request, string token)
        {
            var response = await SendRequestAsync("api/room/join-room", ApiType.POST, request,token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
    }

        public async Task<(bool, string?)> KickMemberAsync(RemoveMemberRequest request, string token)
        {
            var response = await SendRequestAsync("api/room/kick-member", ApiType.POST, request,token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
    }

        public async Task<(bool, string?)> LeaveRoomAsync(LeaveRoomRequest request, string token)
        {
            var response = await SendRequestAsync($"api/room/leave-room", ApiType.POST, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
    }

        public async Task<List<MessageModel>> GetMessageHistoryAsync([FromQuery] GetHistoryRequest request, string token)
        {
            var response = await SendRequestAsync($"api/room/message-history", ApiType.GET, request, token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ConvertTo<List<MessageModel>>() ?? new List<MessageModel>();
            }
            return new List<MessageModel>();
        }

        public async Task<List<ChatUserResponse>> GetRoomUserAsync([FromQuery] GetMemberInGroupRequest request, string token)
        {
            var response = await SendRequestAsync($"api/room/get-members", ApiType.GET, request, token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ConvertTo<List<ChatUserResponse>>() ?? new List<ChatUserResponse>();
            }
            return new List<ChatUserResponse>();
        }

        
    }
}
