using Microsoft.AspNetCore.Http;

namespace HUBT_Social_Chat_Service.Interfaces
{
    public interface IFileUploadService
    {
        Task<bool> UploadFileAsync(IFormFile file);
    }
}
