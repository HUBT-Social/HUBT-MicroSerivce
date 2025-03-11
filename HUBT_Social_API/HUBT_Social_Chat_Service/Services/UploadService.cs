using HUBT_Social_Chat_Resources.Dtos.Collections;
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Extention;
using HUBT_Social_Chat_Service.Helper;
using HUBT_Social_Chat_Service.Interfaces;
using MongoDB.Driver;
using System.Threading.Channels;

namespace HUBT_Social_Chat_Service.Services
{
    public class UploadService : IUploadService
    {
        private readonly IMessageUploadService _messageUploadService;
        private readonly IMediaUploadService _mediaUploadService;
        //private readonly IFileUploadService _fileUploadService;

        public UploadService(
            IMessageUploadService messageUploadService,
            IMediaUploadService mediaUploadService
            /*IFileUploadService fileUploadService*/)
        {
            _messageUploadService = messageUploadService;
            _mediaUploadService = mediaUploadService;
           // _fileUploadService = fileUploadService;
        }

        public async Task SendChatAsync(ChatRequest chatRequest, IMongoCollection<ChatGroupModel> collection, Channel<(bool, MessageModel?)> channel)
        {
            var tasks = new List<Task>();

            ReplyMessage? replyMessage = null;

            // Xử lý tin nhắn trả lời
            if (chatRequest.ReplyToMessageId is not null)
            {
                MessageModel? messageResult = await collection.GetInfoMessageAsync(chatRequest.GroupId, chatRequest.ReplyToMessageId);
                if (messageResult is not null)
                {
                    replyMessage = new ReplyMessage
                    {
                        message = messageResult.message ?? null,
                        replyBy = chatRequest.UserId,
                        replyTo = messageResult.sentBy,
                        messageType = messageResult.messageType,
                        voiceMessageDuration = messageResult.voiceMessageDuration ?? null,
                        messageId = messageResult.id,
                    };
                }
            }

            // Gửi tin nhắn văn bản nếu có
            if (!string.IsNullOrWhiteSpace(chatRequest.Content))
            {
                var messageRequest = new MessageRequest
                {
                    GroupId = chatRequest.GroupId,
                    Content = chatRequest.Content,
                    UserId = chatRequest.UserId,
                    ReplyToMessage = replyMessage
                };

                tasks.Add(Task.Run(async () =>
                {
                    var result = await _messageUploadService.UploadMessageAsync(messageRequest, collection);
                    await channel.Writer.WriteAsync(result); // Gửi về client ngay khi xong
                }));
            }

            // Gửi media (ảnh, video)
            if (chatRequest.Medias is { Count: > 0 })
            {
                var mediaRequest = new MediaRequest
                {
                    GroupId = chatRequest.GroupId,
                    Medias = chatRequest.Medias,
                    UserId = chatRequest.UserId,
                    ReplyToMessage = replyMessage
                };

                tasks.Add(Task.Run(async () =>
                {
                    var result = await _mediaUploadService.UploadMediaAsync(mediaRequest, collection);
                    await channel.Writer.WriteAsync(result); // Gửi về client ngay khi xong
                }));
            }

            // Đợi tất cả tác vụ hoàn thành
            await Task.WhenAll(tasks);

            // Đóng channel khi xong
            channel.Writer.Complete();
        }

    }
}
