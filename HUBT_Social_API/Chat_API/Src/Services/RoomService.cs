
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
using HUBT_Social_Core.Settings;

namespace Chat_API.Src.Services
{
    public class RoomService(IHttpService httpService, string basePath) : BaseService(httpService,basePath), IRoomService
    {


        public async Task<(bool,string?)> UpdateGroupNameAsync(UpdateGroupNameRequest request, string token)
        {
            var response = await SendRequestAsync(APIEndPoint.ChatDataUrls.Put_Update_Group_Name, ApiType.PUT, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
        }

        //public async Task<bool> UpdateAvatarAsync(string groupId, string userId, string newUrl, string token);
        //{
        //    var response = await SendRequestAsync($"api/room/update-avatar?groupId={groupId}&newUrl={newUrl}", ApiType.PUT);
        //    return response.StatusCode == System.Net.HttpStatusCode.OK;
        //}

        public async Task<(bool, string?)> UpdateNickNameAsync(UpdateNickNameRequest request, string token)
    {
            var response = await SendRequestAsync(APIEndPoint.ChatDataUrls.Put_Update_Nick_Name, ApiType.PUT, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
        }

        public async Task<(bool, string?)> UpdateParticipantRoleAsync(UpdateParticipantRoleRequest request, string token)
        { 
            var response = await SendRequestAsync(APIEndPoint.ChatDataUrls.Put_Update_Participant_Role, ApiType.PUT, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
        }


        public async Task<(bool, string?)> JoinRoomAsync(AddMemberRequestData request, string token)
        {
            var response = await SendRequestAsync(APIEndPoint.ChatDataUrls.Post_Join_Group, ApiType.POST, request,token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
    }

        public async Task<(bool, string?)> KickMemberAsync(RemoveMemberRequest request, string token)
        {
            var response = await SendRequestAsync(APIEndPoint.ChatDataUrls.Post_Kick_Member, ApiType.POST, request,token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
    }

        public async Task<(bool, string?)> LeaveRoomAsync(LeaveRoomRequest request, string token)
        {
            var response = await SendRequestAsync(APIEndPoint.ChatDataUrls.Post_Leave_Group, ApiType.POST, request, token);
            return (response.StatusCode == System.Net.HttpStatusCode.OK, response.Message?.ToString());
    }

        //Đang gặp bug convert thì bị null. 
        // có check null chó đâu thì bị null là đúng r
        public async Task<MessageResponse<List<MessageDTO>>?> GetMessageHistoryAsync(GetHistoryRequest request, string token)
        {
            string path = APIEndPoint.ChatDataUrls.Get_History_Content
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        { "ChatRoomId", request.ChatRoomId ?? string.Empty },
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
            string path = APIEndPoint.ChatDataUrls.Get_Members
                .BuildUrl(
                    new Dictionary<string, string>
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
