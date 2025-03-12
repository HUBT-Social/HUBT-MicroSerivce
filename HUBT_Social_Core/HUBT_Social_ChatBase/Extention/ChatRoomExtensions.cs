
using ChatBase.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Base.Extention
{
    public static class ChatRoomExtensions
    {
        /// <summary>
        /// Gets the list of chat room IDs where the user is a participant.
        /// </summary>
        public static async Task<List<string>> GetUserGroupConnectedAsync(this IMongoCollection<ChatRoomModel> chatRooms, string userId)
        {
            if (chatRooms == null)
                throw new ArgumentNullException(nameof(chatRooms), "Chat rooms collection cannot be null.");

            try
            {
                var filter = Builders<ChatRoomModel>.Filter.ElemMatch(
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
        public static async Task<string?> GetRoleAsync(this IMongoCollection<ChatRoomModel> chatRooms, string roomId, string userId)
        {
            if (chatRooms == null)
                throw new ArgumentNullException(nameof(chatRooms), "Chat rooms collection cannot be null.");

            try
            {
                var filter = Builders<ChatRoomModel>.Filter.And(
                    Builders<ChatRoomModel>.Filter.Eq(r => r.Id, roomId),
                    Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserId == userId)
                );

                var projection = Builders<ChatRoomModel>.Projection.Expression(r =>
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
        public static async Task<string?> GetNickNameAsync(this IMongoCollection<ChatRoomModel> chatRooms, string roomId, string userId)
        {
            if (chatRooms == null)
                throw new ArgumentNullException(nameof(chatRooms), "Chat rooms collection cannot be null.");

            try
            {
                var filter = Builders<ChatRoomModel>.Filter.And(
                    Builders<ChatRoomModel>.Filter.Eq(r => r.Id, roomId),
                    Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserId == userId)
                );

                var projection = Builders<ChatRoomModel>.Projection.Expression(r =>
                    r.Participant.FirstOrDefault(p => p.UserId == userId));

                var participant = await chatRooms
                    .Find(filter)
                    .Project(projection)
                    .FirstOrDefaultAsync();

                return participant?.NickName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving nickname for user {userId} in room {roomId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the information of a specific message in a chat room.
        /// </summary>
        public static async Task<MessageModel?> GetInfoMessageAsync(this IMongoCollection<ChatRoomModel> chatRooms, string roomId, string messageId)
        {
            if (chatRooms == null)
                throw new ArgumentNullException(nameof(chatRooms), "Chat rooms collection cannot be null.");

            try
            {
                var filter = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, roomId);
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