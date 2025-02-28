using HUBT_Social_API.Features.Chat.DTOs;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IUploadChatServices
{
    Task<bool> SendChatAsync(ChatRequest chatRequest);

}