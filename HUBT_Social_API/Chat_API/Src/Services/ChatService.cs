
using Chat_API.Src.Interfaces;
using HUBT_Social_Base.Service;
using HUBT_Social_Base;
using HUBT_Social_Core.Settings.@enum;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Dtos.Request.InitRequest;
using Newtonsoft.Json.Linq;



namespace Chat_API.Src.Services
{
    public class ChatService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IChatService
    {

        public async Task<string?> CreateGroupAsync(CreateGroupRequest createGroupRequest,string token)
        {
            var response = await SendRequestAsync("api/chat/create-group", ApiType.POST, createGroupRequest, token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data?.ToString();
            }
            return "Lỗi khi tạo";
        }

        public async Task<string?> DeleteGroupAsync(string groupId, string token)
        {
            var response = await SendRequestAsync($"api/chat/delete-group?groupId={groupId}", ApiType.DELETE, null, token);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data?.ToString();
            }
            return response.Data?.ToString();
        }

        public async Task<List<GroupSearchResponse>> SearchGroupsAsync(string keyword, int page, int limit, string token)
        {
            var response = await SendRequestAsync($"api/chat/search-groups?keyword={keyword}&page={page}&limit={limit}", ApiType.GET, null, token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ConvertTo<List<GroupSearchResponse>>() ?? new List<GroupSearchResponse>();
            }
            return new List<GroupSearchResponse>();
        }

        public async Task<List<GroupSearchResponse>> GetAllRoomsAsync(int page, int limit, string token)
        {
            var response = await SendRequestAsync($"api/chat/get-all-rooms?page={page}&limit={limit}", ApiType.GET,null,token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ConvertTo<List<GroupSearchResponse>>() ?? new List<GroupSearchResponse>();
            }
            return new List<GroupSearchResponse>();
        }

        public async Task<List<GroupLoadingResponse>> GetRoomsOfUserIdAsync(int page, int limit,string token)
        {
            var response = await SendRequestAsync($"api/chat/get-rooms-of-user?page={page}&limit={limit}", ApiType.GET,null, token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ConvertTo<List<GroupLoadingResponse>>() ?? new List<GroupLoadingResponse>();
            }
            return new List<GroupLoadingResponse>();
        }

        
    }
}
