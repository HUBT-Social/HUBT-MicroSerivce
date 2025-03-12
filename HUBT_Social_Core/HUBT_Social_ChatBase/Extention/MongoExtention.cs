
using ChatBase.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Base.Extention
{
    public static class MongoExtention
    {
        /// <summary>
        /// Saves a message to a chat room and updates the last interaction time.
        /// </summary>
        /// <param name="chatRooms">The MongoDB collection of chat rooms.</param>
        /// <param name="roomId">The ID of the chat room.</param>
        /// <param name="message">The message to save.</param>
        /// <returns>The result of the update operation.</returns>
        public static async Task<UpdateResult> SaveChatItemAsync(this IMongoCollection<ChatRoomModel> chatRooms, string roomId, MessageModel message)
        {
            if (chatRooms == null)
                throw new ArgumentNullException(nameof(chatRooms), "Chat rooms collection cannot be null.");
            if (string.IsNullOrEmpty(roomId))
                throw new ArgumentNullException(nameof(roomId), "Room ID cannot be null or empty.");
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");

            try
            {
                // 📌 Tạo bộ lọc tìm phòng chat theo `Room.Id`
                var filter = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, roomId);
                // Nếu cần kiểm tra `sentBy` trong `Participant`, có thể thêm lại:
                // Builders<ChatRoomModel>.Filter.ElemMatch(cr => cr.Participant, p => p.UserName == message.SentBy)

                // ✅ Cập nhật `LastInteractionTime`
                var updateLastInteractionTime = Builders<ChatRoomModel>.Update
                    .Set(cr => cr.LastInteractionTime, DateTime.UtcNow);

                // ✅ Thêm tin nhắn mới vào `Content`
                var updateChatItems = Builders<ChatRoomModel>.Update
                    .Push(cr => cr.Content, message);

                // 🛠 Kết hợp các cập nhật và thực hiện lệnh UpdateOne
                var updateResult = await chatRooms.UpdateOneAsync(
                    filter,
                    Builders<ChatRoomModel>.Update.Combine(updateLastInteractionTime, updateChatItems)
                );

                // ✅ In kết quả để kiểm tra
                Console.WriteLine($"🔹 MatchedCount: {updateResult.MatchedCount}, ModifiedCount: {updateResult.ModifiedCount}");

                if (updateResult.ModifiedCount > 0)
                {
                    Console.WriteLine("✅ Dữ liệu đã được cập nhật.");
                }
                else
                {
                    Console.WriteLine("⚠️ Không có dữ liệu nào được cập nhật. Kiểm tra lại `filter` hoặc `update`.");
                }

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