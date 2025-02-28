
namespace HUBT_Social_API.Features.Chat.Services.Interfaces;
public interface IFileUploadService
{
    Task<bool> UploadFileAsync(IFormFile file);
}