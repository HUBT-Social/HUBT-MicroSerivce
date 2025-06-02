using HUBT_Social_Base;
using HUBT_Social_Core.Models.Requests.Chat;

namespace User_API.Src.Service
{
    public interface IChatService : IBaseService
    {
        Task<CreateChatResponse> CreateChatRoom(CreateGroupRequest request, string accessToken);

    }
}
