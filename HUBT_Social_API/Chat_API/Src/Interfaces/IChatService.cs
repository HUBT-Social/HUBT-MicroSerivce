using HUBT_Social_Base;
using HUBT_Social_Chat_Resources.Dtos.Request.InitRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Core.Models.DTOs;

namespace Chat_API.Src.Interfaces
{
    public interface IChatService : IBaseService
    {
        Task<ResponseDTO> CreateGroupAsync(CreateGroupRequestData createGroupRequest, string token);
        Task<ResponseDTO> DeleteGroupAsync(string idGroup, string token);
        Task<List<GroupSearchResponse>> SearchGroupsAsync(string keyword, int page, int limit,string token);
        Task<List<GroupSearchResponse>> GetAllRoomsAsync(int page, int limit,string token);
        Task<List<GroupLoadingResponse>> GetRoomsOfUserIdAsync(int page, int limit,string token);

    }
}
