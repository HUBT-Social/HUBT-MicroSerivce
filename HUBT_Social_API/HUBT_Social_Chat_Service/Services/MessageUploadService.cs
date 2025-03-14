
using HtmlAgilityPack;
using HUBT_Social_Chat_Resources.Dtos.Collections;
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Helper;
using HUBT_Social_Chat_Service.Interfaces;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using HUBT_Social_Chat_Service.Extention;
using HUBT_Social_MongoDb_Service.Services;
using HUBT_Social_MongoDb_Service.ASP_Extentions;

namespace HUBT_Social_Chat_Service.Services
{
    public class MessageUploadService : IMessageUploadService
    {

        public async Task<(bool Success, MessageModel? Message)> UploadMessageAsync(
                MessageRequest chatRequest, IMongoService<ChatGroupModel> _chatRooms)
        {
            // Lấy ChatRoom từ MongoDB
            var filterGetChatRoom = Builders<ChatGroupModel>.Filter.Eq(cr => cr.Id, chatRequest.GroupId);
            ChatGroupModel? chatRoom = await _chatRooms.Find(cr => cr.Id == chatRequest.GroupId).FirstOrDefaultAsync();

            if (chatRoom == null)
            {
                return (false, null); // Không có chatroom, trả về false và null
            }

            var links = ExtractLinksIfPresent(chatRequest.Content);
            MessageContent MessageContent = new(chatRequest.Content);

            if (links.Count > 0)
            {
                foreach (var link in links)
                {
                    var metadata = await FetchLinkMetadataAsync(link);
                    if (metadata != null)
                    {
                        MessageContent.Links.Add(metadata);
                    }
                }
            }

            // Tạo tin nhắn
            MessageModel message = await MessageModel.CreateTextMessageAsync(
                chatRequest.UserId, MessageContent.Content, chatRequest.RequestId, chatRequest.ReplyToMessage);

            bool updateResult = await _chatRooms.SaveChatItemAsync(chatRoom.Id, message);
            return (updateResult, message);
        }


        private async Task<LinkMetadataModel?> FetchLinkMetadataAsync(string url)
        {
            try
            {
                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; FetchBot/1.0)");

                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch URL {url}. Status code: {response.StatusCode}");
                    return null;
                }

                var htmlContent = await response.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim();
                var thumbnail = doc.DocumentNode
                    .SelectSingleNode("//meta[@property='og:image']")?
                    .GetAttributeValue("content", "");
                var description = doc.DocumentNode
                    .SelectSingleNode("//meta[@name='description']")?
                    .GetAttributeValue("content", "") ??
                    doc.DocumentNode.SelectSingleNode("//meta[@property='og:description']")?
                    .GetAttributeValue("content", "");

                return new LinkMetadataModel
                {
                    Url = url,
                    Title = title ?? "No Title",
                    Thumbnail = thumbnail,
                    Description = description ?? "No Description"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch metadata for {url}: {ex.Message}");
                return null;
            }
        }
        private List<string> ExtractLinksIfPresent(string message)
        {
            if (!Regex.IsMatch(message, @"(http|https):\/\/[^\s]+|www\.[^\s]+"))
            {
                return new List<string>(); // Không có link
            }

            // Xử lý tách link nếu phát hiện
            var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var links = new List<string>();

            foreach (var word in words)
            {
                if (Uri.TryCreate(word, UriKind.Absolute, out Uri? uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    // Thêm link vào danh sách
                    links.Add(word);

                }

            }

            return links;
        }
    }
}
