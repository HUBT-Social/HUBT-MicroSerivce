using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_MongoDb_Service.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Service.Helper
{
    public static class ChatRoomHelper
    {
        public static async Task<List<string>> GetUserGroupConnectedAsync(this IMongoService<ChatGroupModel> chatGroups, string userId)
        {

            try
            {
                if (chatGroups == null)
                    throw new ArgumentNullException(nameof(chatGroups), "Chat groups service cannot be null.");

                var groups = await chatGroups.Find(group => group.Participant.Any(p => p.UserId == userId));

                var roomIds = groups.Select(g => g.Id).ToList();

                return roomIds??new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving chat groups for user {userId}: {ex.Message}");
                return new List<string>();
            }
        }

        public static async Task<string?> GetRoleAsync(this IMongoService<ChatGroupModel> chatGroups, string roomId, string userId)
        {
            try
            {
                if (chatGroups == null)
                    throw new ArgumentNullException(nameof(chatGroups), "Chat groups service cannot be null.");

                // Tìm nhóm chat có roomId và có userId trong danh sách Participant
                var group = await chatGroups.Find(g => g.Id == roomId && g.Participant.Any(p => p.UserId == userId));

                // Lấy participant của userId trong nhóm chat đó
                var participant = group.FirstOrDefault()?.Participant.FirstOrDefault(p => p.UserId == userId);

                return participant?.Role.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving role for user {userId} in room {roomId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the nickname of a user in a specific chat room.
        /// </summary>
        public static async Task<string?> ToName(this string userId, ChatGroupModel chatGroups)
        {

            try
            {
                Participant? user = chatGroups.Participant.Find(p => p.UserId == userId);

                return user is not null ? user.NickName : "Anonymus";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving nickname for user {userId} in room: {ex.Message}");
                return "Anonymus";
            }
        }


        /// <summary>
        /// Gets the information of a specific message in a chat room.
        /// </summary>
        public static async Task<MessageModel?> GetInfoMessageAsync(this IMongoService<ChatGroupModel> chatGroups, string roomId, string messageId)
        {
            try
            {
                if (chatGroups == null)
                    throw new ArgumentNullException(nameof(chatGroups), "Chat groups service cannot be null.");

                // Tìm nhóm chat có roomId
                var group = await chatGroups.Find(g => g.Id == roomId);

                // Lấy nhóm chat đầu tiên (vì Id là duy nhất, thường chỉ có 1 kết quả)
                var chatRoom = group.FirstOrDefault();

                // Nếu không tìm thấy nhóm hoặc danh sách Content rỗng, trả về null
                if (chatRoom?.Content == null)
                    return null;

                // Tìm tin nhắn có messageId trong danh sách Content
                var message = chatRoom.Content.FirstOrDefault(ci => ci.id == messageId);
                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving message {messageId} info in room {roomId}: {ex.Message}");
                return null;
            }
        }
    }
}

