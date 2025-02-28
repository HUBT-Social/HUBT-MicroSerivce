
using HUBT_Social_API.Features.Chat.Services.Interfaces;

public class FileUploadService : IFileUploadService
{
    public async Task<bool> UploadFileAsync(IFormFile file)
    {

        return true;
    }
}