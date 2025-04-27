using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.Requests.Chat;
using HUBT_Social_Core.Settings.@enum;

namespace User_API.Src.Service
{
    public class ChatService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IChatService
    {
        public async Task<bool> CreateChatRoom(CreateGroupRequest request,string accessToken)
        {
            string path = "api/chat/create-group";
            HttpResponseMessage httpResponse = await SendActionResultRequestAsync(path, ApiType.POST, request, accessToken);
            return httpResponse.IsSuccessStatusCode;
        }
    }
}
