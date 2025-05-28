
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
using HUBT_Social_Core.Settings;



namespace Chat_API.Src.Services
{
    public class ChatService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IChatService
    {

        public async Task<ResponseDTO> CreateGroupAsync(CreateGroupRequestData createGroupRequest,string token)
        {
            return  await SendRequestAsync(APIEndPoint.ChatDataUrls.PostCreateGroup, ApiType.POST, createGroupRequest, token);
        }

        public async Task<ResponseDTO> DeleteGroupAsync(string groupId, string token)
        {
            string path = APIEndPoint.ChatDataUrls.DeleteGroup
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        { "groupId", groupId }
                    }
                );
            return await SendRequestAsync(path, ApiType.DELETE, null, token);
        }

        public async Task<List<GroupSearchResponse>> SearchGroupsAsync(string keyword, int page, int limit, string token)
        {
            string path = APIEndPoint.ChatDataUrls.GetSearchGroup
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        { "keyword", keyword },
                        { "page", page.ToString() },
                        { "limit", limit.ToString() }
                    }
                );
            var response = await SendRequestAsync(path, ApiType.GET, null, token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ConvertTo<List<GroupSearchResponse>>() ?? [];
            }
            return [];
        }

        public async Task<List<GroupSearchResponse>> GetAllRoomsAsync(int page, int limit, string token)
        {
            string path = APIEndPoint.ChatDataUrls.GetAllGroup
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        { "page", page.ToString() },
                        { "limit", limit.ToString() }
                    }
                );
            try
            {
                var response = await SendRequestAsync(path, ApiType.GET, null, token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.ConvertTo<List<GroupSearchResponse>>() ?? [];
                }
            }
            catch { }
            
            return [];
        }

        public async Task<List<GroupLoadingResponse>> GetRoomsOfUserAsync(int page, int limit,string token)
        {
            string path = APIEndPoint.ChatDataUrls.GetUserGroup
                .BuildUrl(
                    new Dictionary<string, string>
                    {
                        { "page", page.ToString() },
                        { "limit", limit.ToString() }
                    }
                );
            try 
            {
                var response = await SendRequestAsync(path, ApiType.GET, null, token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.ConvertTo<List<GroupLoadingResponse>>() ?? [];
                }
            } catch { }
            
            return [];
        }

 
    }
}
