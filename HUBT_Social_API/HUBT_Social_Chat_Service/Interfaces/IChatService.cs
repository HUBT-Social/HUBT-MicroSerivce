using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Service.Interfaces
{
    public interface IChatService
    {
        Task<(bool, string)> CreateGroupAsync(ChatGroupModel newRoomModel);
        Task<(bool, string)> DeleteGroupAsync(string idGroup);
        Task<List<GroupSearchResponse>> SearchGroupsAsync(string keyword, int page, int limit);
        Task<List<GroupSearchResponse>> GetAllRoomsAsync(int page, int limit);
        Task<List<GroupLoadingResponse>> GetRoomsOfUserAsync(string userId, int page, int limit);
        Task<ChatGroupModel?> GetGroupById(string groupId);

    }
}
