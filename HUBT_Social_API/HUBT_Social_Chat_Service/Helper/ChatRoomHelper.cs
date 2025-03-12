using HUBT_Social_Chat_Resources.Models;
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
        public static async Task<List<string>> GetUserGroupConnectedAsync(this IMongoCollection<ChatGroupModel> chatRooms, string userId)
        {
            if (chatRooms == null)
                throw new ArgumentNullException(nameof(chatRooms), "Chat rooms collection cannot be null.");

            try
            {
                var filter = Builders<ChatGroupModel>.Filter.ElemMatch(
                    cr => cr.Participant,
                    p => p.UserId == userId
                );

                var roomIds = await chatRooms
                    .Find(filter)
                    .Project(cr => cr.Id)
                    .ToListAsync();

                return roomIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving chat groups for user {userId}: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Gets the role of a user in a specific chat room.
        /// </summary>
        public static async Task<string?> GetRoleAsync(this IMongoCollection<ChatGroupModel> chatRooms, string roomId, string userId)
        {
            if (chatRooms == null)
                throw new ArgumentNullException(nameof(chatRooms), "Chat rooms collection cannot be null.");

            try
            {
                var filter = Builders<ChatGroupModel>.Filter.And(
                    Builders<ChatGroupModel>.Filter.Eq(r => r.Id, roomId),
                    Builders<ChatGroupModel>.Filter.ElemMatch(r => r.Participant, p => p.UserId == userId)
                );

                var projection = Builders<ChatGroupModel>.Projection.Expression(r =>
                    r.Participant.FirstOrDefault(p => p.UserId == userId));

                var participant = await chatRooms
                    .Find(filter)
                    .Project(projection)
                    .FirstOrDefaultAsync();

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
        public static async Task<MessageModel?> GetInfoMessageAsync(this IMongoCollection<ChatGroupModel> chatRooms, string roomId, string messageId)
        {
            if (chatRooms == null)
                throw new ArgumentNullException(nameof(chatRooms), "Chat rooms collection cannot be null.");

            try
            {
                var filter = Builders<ChatGroupModel>.Filter.Eq(cr => cr.Id, roomId);
                var chatRoom = await chatRooms.Find(filter).FirstOrDefaultAsync();

                if (chatRoom?.Content == null)
                    return null;

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

