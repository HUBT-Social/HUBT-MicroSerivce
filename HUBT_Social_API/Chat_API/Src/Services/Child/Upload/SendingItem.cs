using FireSharp.Extensions;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.Services.Child
{
    public static class SendingItem
    {
        // Gửi tin nhắn hoặc phương tiện đến nhóm
        public static async Task SendChatItem(string groupId, MessageModel chatItem,IHubContext<ChatHub> _hubContext)
        {
            try
            {
                Console.WriteLine("giui tin: ",chatItem.messageType);
                MessageResponse messageResponse = new MessageResponse
                {
                    groupId = groupId,
                    message = chatItem,
                };
                await _hubContext.Clients.Group(groupId).SendAsync("ReceiveChat", messageResponse);
                Console.WriteLine("14");
            }
            catch (Exception ex)
            {
                // Log lỗi và xử lý tùy theo nhu cầu
                Console.WriteLine($"Error sending ReceiveChat: {ex.Message}");
            }
        }
    }
}