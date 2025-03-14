

using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_MongoDb_Service.Services;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Service.Extention
{
    public static class MongoExtention
    {

        public static async Task<bool> SaveChatItemAsync(this IMongoService<ChatGroupModel> chatRooms, string roomId, MessageModel message)
        {
            try
            {

                if (chatRooms == null)
                    throw new ArgumentNullException(nameof(chatRooms), "Chat rooms collection cannot be null.");
                if (string.IsNullOrEmpty(roomId))
                    throw new ArgumentNullException(nameof(roomId), "Room ID cannot be null or empty.");
                if (message == null)
                    throw new ArgumentNullException(nameof(message), "Message cannot be null.");

                Expression<Func<ChatGroupModel, bool>> filter = cr => cr.Id == roomId;

                var updateLastInteractionTime = Builders<ChatGroupModel>.Update
                    .Set(cr => cr.LastInteractionTime, DateTime.UtcNow);
                var updateChatItems = Builders<ChatGroupModel>.Update
                    .Push(cr => cr.Content, message);

                var updateResult = await chatRooms.UpdateByFilter(filter, Builders<ChatGroupModel>.Update.Combine(updateLastInteractionTime, updateChatItems));

                return updateResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi lưu tin nhắn: {ex.Message}");
                throw;
            }
        }
    }
}