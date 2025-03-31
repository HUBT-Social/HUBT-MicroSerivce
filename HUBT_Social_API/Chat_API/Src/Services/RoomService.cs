
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
using MongoDB.Bson;
using HUBT_Social_Core.Models.DTOs;
using System.Collections.Generic;
using System.Text.Json;
using Chat_API.Src.Constants;
using HUBT_Social_Core.ASP_Extensions;

namespace Chat_API.Src.Services
{
    public class RoomService(IHttpService httpService, string basePath) : BaseService(httpService,basePath), IRoomService
    {


        public async Task<(bool,string?)> UpdateGroupNameAsync(UpdateGroupNameRequest request, string token)
        {
            var response = await SendRequestAsync(ChatApiEndpoints.RoomService_UpdateGroupName, ApiType.PUT, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
        }

        //public async Task<bool> UpdateAvatarAsync(string groupId, string userId, string newUrl, string token);
        //{
        //    var response = await SendRequestAsync($"api/room/update-avatar?groupId={groupId}&newUrl={newUrl}", ApiType.PUT);
        //    return response.StatusCode == System.Net.HttpStatusCode.OK;
        //}

        public async Task<(bool, string?)> UpdateNickNameAsync(UpdateNickNameRequest request, string token)
    {
            var response = await SendRequestAsync(ChatApiEndpoints.RoomService_UpdateNickName, ApiType.PUT, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
        }

        public async Task<(bool, string?)> UpdateParticipantRoleAsync(UpdateParticipantRoleRequest request, string token)
        { 
            var response = await SendRequestAsync(ChatApiEndpoints.RoomService_UpdateParticipantRole, ApiType.PUT, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
        }


        public async Task<(bool, string?)> JoinRoomAsync(AddMemberRequestData request, string token)
        {
            var response = await SendRequestAsync(ChatApiEndpoints.RoomService_JoinRoom, ApiType.POST, request,token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
    }

        public async Task<(bool, string?)> KickMemberAsync(RemoveMemberRequest request, string token)
        {
            var response = await SendRequestAsync(ChatApiEndpoints.RoomService_KickMember, ApiType.POST, request,token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
    }

        public async Task<(bool, string?)> LeaveRoomAsync(LeaveRoomRequest request, string token)
        {
            var response = await SendRequestAsync(ChatApiEndpoints.RoomService_LeaveRoom, ApiType.POST, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
    }

        //Đang gặp bug convert thì bị null.
        public async Task<MessageResponse<List<MessageDTO>>?> GetMessageHistoryAsync(GetHistoryRequest request, string token)
        {
            string path = ChatApiEndpoints.RoomService_GetMessageHistory
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        { "ChatRoomId", Uri.EscapeDataString(request.ChatRoomId ?? string.Empty) },
                        { "CurrentQuantity", request.CurrentQuantity.ToString()??"0" },
                        { "Limit", request.Limit.ToString()??"20" },
                        { "Type", request.Type.ToString()??"-1" }
                    }
                );

            // Gửi yêu cầu API
            ResponseDTO? response = await SendRequestAsync(path, ApiType.GET, null, token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ConvertTo<MessageResponse<List<MessageDTO>>>() ?? new MessageResponse<List<MessageDTO>>();
            }
            return new MessageResponse<List<MessageDTO>>();
        }
        public async Task<GetMemberGroup> GetRoomUserAsync(GetMemberInGroupRequest request, string token)
        {
            string path = ChatApiEndpoints.RoomService_GetRoomUser
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        { "groupId", request.groupId }
                    }
                );
            var response = await SendRequestAsync(path, ApiType.GET, null, token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ConvertTo<GetMemberGroup>() ?? new GetMemberGroup();
            }
            return new GetMemberGroup();
        }

    }
}
