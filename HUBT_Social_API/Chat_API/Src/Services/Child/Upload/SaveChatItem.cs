
using System.Text.Json;
using FireSharp.Extensions;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services.Child
{
    public static class SaveChatItem
{
    public static async Task<UpdateResult> Save(IMongoCollection<ChatRoomModel> chatRooms, string RoomId, MessageModel Message)
    {
        try
        {
            // 📌 Tạo bộ lọc tìm phòng chat theo `Room.Id` và đảm bảo `UserName` có trong `Participant`
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, RoomId)
                //Builders<ChatRoomModel>.Filter.ElemMatch(cr => cr.Participant, p => p.UserName == Message.sentBy)
            );

   
            // ✅ Cập nhật `LastInteractionTime`
            var updateLastInteractionTime = Builders<ChatRoomModel>.Update
                .Set(cr => cr.LastInteractionTime, DateTime.UtcNow);

            // ✅ Thêm tin nhắn mới vào `Content`
            var updateChatItems = Builders<ChatRoomModel>.Update
                .Push(cr => cr.Content, Message);



            // 🛠 Chạy update thứ hai để `Push` tin nhắn mới
            var updateResult = await chatRooms.UpdateOneAsync(filter, Builders<ChatRoomModel>.Update.Combine(updateLastInteractionTime, updateChatItems));

            // ✅ In kết quả để kiểm tra
            Console.WriteLine($"🔹 MatchedCount: {updateResult.MatchedCount}, ModifiedCount: {updateResult.ModifiedCount}");

            if (updateResult.ModifiedCount > 0)
            {
                Console.WriteLine("✅ Dữ liệu đã đượcđược cập nhật: ");
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