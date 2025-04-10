
using Chat_API.Src.Interfaces;
using HUBT_Social_Base.Service;
using HUBT_Social_Base;
using HUBT_Social_Core.Settings.@enum;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Dtos.Request.InitRequest;
using Newtonsoft.Json.Linq;
using HUBT_Social_Core.Models.DTOs;
using Chat_API.Src.Constants;
using HUBT_Social_Core.ASP_Extensions;
using System.Text.RegularExpressions;
using Amazon.Runtime.Internal.Transform;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;



namespace Chat_API.Src.Services
{
    public class ChatService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IChatService
    {

        public async Task<ResponseDTO> CreateGroupAsync(CreateGroupRequestData createGroupRequest,string token)
        {
            return  await SendRequestAsync(ChatApiEndpoints.ChatService_CreateGroup, ApiType.POST, createGroupRequest, token);
        }

        public async Task<ResponseDTO> DeleteGroupAsync(string groupId, string token)
        {
            string path = ChatApiEndpoints.ChatService_DeleteGroup
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        { "groupId", groupId }
                    }
                );
            return await SendRequestAsync(path, ApiType.DELETE, null, token);
        }

        public async Task<List<GroupSearchResponse>> SearchGroupsAsync(string keyword, int page, int limit, string token)
        {
            string path = ChatApiEndpoints.ChatService_SearchGroups
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        { "keyword", keyword },
                        { "page", page },
                        { "limit", limit }
                    }
                );
            var response = await SendRequestAsync(path, ApiType.GET, null, token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ConvertTo<List<GroupSearchResponse>>() ?? new List<GroupSearchResponse>();
            }
            return new List<GroupSearchResponse>();
        }

        public async Task<List<GroupSearchResponse>> GetAllRoomsAsync(int page, int limit, string token)
        {
            string path = ChatApiEndpoints.ChatService_GetAllRooms
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        { "page", page },
                        { "limit", limit }
                    }
                );
            try
            {
                var response = await SendRequestAsync(path, ApiType.GET, null, token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.ConvertTo<List<GroupSearchResponse>>() ?? new List<GroupSearchResponse>();
                }
            }
            catch { }
            
            return new List<GroupSearchResponse>();
        }

        public async Task<List<GroupLoadingResponse>> GetRoomsOfUserIdAsync(int page, int limit,string token)
        {
            string path = ChatApiEndpoints.ChatService_GetRoomsOfUser
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        { "page", page },
                        { "limit", limit }
                    }
                );
            try 
            {
                var response = await SendRequestAsync(path, ApiType.GET, null, token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.ConvertTo<List<GroupLoadingResponse>>() ?? new List<GroupLoadingResponse>();
                }
            } catch { }
            
            return new List<GroupLoadingResponse>();
        }

 
    }
}
