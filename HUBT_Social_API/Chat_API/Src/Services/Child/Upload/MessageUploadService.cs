
using System.Text.RegularExpressions;
using Amazon.SecurityToken.Model;
using HtmlAgilityPack;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Child;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

public class MessageUploadService : IMessageUploadService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    public MessageUploadService(IMongoCollection<ChatRoomModel> chatRooms)
    {
        _chatRooms = chatRooms;
    }
    public async Task<bool> UploadMessageAsync(MessageRequest chatRequest,IHubContext<ChatHub> hubContext)
    {
        Console.WriteLine("11");
        // Lấy ChatRoom từ MongoDB
        FilterDefinition<ChatRoomModel> filterGetChatRoom = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, chatRequest.GroupId);
        ChatRoomModel chatRoom = await _chatRooms.Find(filterGetChatRoom).FirstOrDefaultAsync();

        if(chatRoom == null){return false;}

        var links = ExtractLinksIfPresent(chatRequest.Content);
        
        MessageContent MessageContent = new(chatRequest.Content);


        if(links.Count > 0)
        {
            foreach (var link in links)
            {
                var metadata = await FetchLinkMetadataAsync(link);
                if(metadata != null)
                {
                    MessageContent.Links.Add(metadata);
                }
            }
        }
        Console.WriteLine("12");
        MessageModel message = await MessageModel.CreateTextMessageAsync(chatRequest.UserId,MessageContent.Content,chatRequest.ReplyToMessage);
        Console.WriteLine("13");
        await SendingItem.SendChatItem(chatRequest.GroupId,message,hubContext); 

        UpdateResult updateResult = await SaveChatItem.Save(_chatRooms,chatRoom.Id,message);
        return updateResult.ModifiedCount > 0;
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