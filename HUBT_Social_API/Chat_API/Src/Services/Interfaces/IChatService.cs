using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using static HUBT_Social_API.Features.Chat.Services.ChatService;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IChatService
{
    Task<string?> CreateGroupAsync(ChatRoomModel newRoomModel);
    Task<bool> DeleteGroupAsync(string idGroup);
    Task<List<RoomSearchReponse>> SearchGroupsAsync(string keyword, int page, int limit);
    Task<List<RoomSearchReponse>> GetAllRoomsAsync(int page, int limit);
    Task<List<RoomLoadingRespone>> GetRoomsOfUserIdAsync(string userId, int page, int limit);

}