using HUBT_Social_Chat_Resources.Dtos.Collections;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Helper;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_MongoDb_Service.Services;
using System.Threading.Channels;

namespace HUBT_Social_Chat_Service.Services
{
    public class UploadService : IUploadService
    {
        private readonly IMessageUploadService _messageUploadService;
        private readonly IMediaUploadService _mediaUploadService;

        public UploadService(IMessageUploadService messageUploadService, IMediaUploadService mediaUploadService)
        {
            _messageUploadService = messageUploadService;
            _mediaUploadService = mediaUploadService;
        }

        public async Task SendChatAsync(ChatRequest chatRequest, IMongoService<ChatGroupModel> collection, Channel<(bool, MessageModel?, string)> channel)
        {
            var tasks = new List<Task>();

            // Xử lý tin nhắn trả lời nếu có
            ReplyMessage? replyMessage = null;
            if (!string.IsNullOrEmpty(chatRequest.ReplyToMessageId))
            {
                var messageResult = await collection.GetInfoMessageAsync(chatRequest.GroupId, chatRequest.ReplyToMessageId);
                if (messageResult != null)
                {
                    replyMessage = new ReplyMessage
                    {
                        message = messageResult.message,
                        replyBy = chatRequest.UserName,
                        replyTo = messageResult.sentBy,
                        messageType = messageResult.messageType,
                        voiceMessageDuration = messageResult.voiceMessageDuration,
                        messageId = messageResult.id
                    };
                }
            }

            // Gửi tin nhắn văn bản (nếu có)
            if (!string.IsNullOrWhiteSpace(chatRequest.Content))
            {
                tasks.Add(ProcessMessageAsync(chatRequest, replyMessage, collection, channel));
            }

            // Gửi media (ảnh, video) nếu có
            if (chatRequest.Medias is { Count: > 0 })
            {
                tasks.Add(ProcessMediaAsync(chatRequest, replyMessage, collection, channel));
            }

            // Đợi tất cả nhiệm vụ hoàn thành trước khi đóng channel
            await Task.WhenAll(tasks);
            channel.Writer.Complete();
        }

        private async Task ProcessMessageAsync(ChatRequest chatRequest, ReplyMessage? replyMessage, IMongoService<ChatGroupModel> collection, Channel<(bool, MessageModel?, string)> channel)
        {
            var messageRequest = new MessageRequest
            {
                RequestId = chatRequest.RequestId,
                GroupId = chatRequest.GroupId,
                Content = chatRequest.Content,
                UserId = chatRequest.UserName,
                ReplyToMessage = replyMessage
            };

            var result = await _messageUploadService.UploadMessageAsync(messageRequest, collection);
            await channel.Writer.WriteAsync((result.Item1, result.Item2, chatRequest.RequestId ?? Guid.NewGuid().ToString()));
        }

        private async Task ProcessMediaAsync(ChatRequest chatRequest, ReplyMessage? replyMessage, IMongoService<ChatGroupModel> collection, Channel<(bool, MessageModel?, string)> channel)
        {
            if (chatRequest.Medias is null || !chatRequest.Medias.Any())
            {
                return;
            }

            var uploadTasks = new List<Task>();

            foreach (var media in chatRequest.Medias)
            {
                uploadTasks.Add(Task.Run(async () =>
                {
                    var mediaRequest = new MediaRequest
                    {
                        GroupId = chatRequest.GroupId,
                        Medias = media, // Chỉ upload 1 file
                        UserId = chatRequest.UserName,
                        ReplyToMessage = replyMessage
                    };

                    var result = await _mediaUploadService.UploadMediaAsync(mediaRequest, collection);

                    await channel.Writer.WriteAsync((result.Item1, result.Item2, chatRequest.RequestId ?? Guid.NewGuid().ToString()));
                }));
            }

            await Task.WhenAll(uploadTasks);
            channel.Writer.Complete(); // Đánh dấu channel hoàn thành
        }
    }
}
