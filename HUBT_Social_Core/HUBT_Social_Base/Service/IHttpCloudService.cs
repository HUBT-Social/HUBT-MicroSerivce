
using HUBT_Social_Base.Models;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests.Cloud;
using Microsoft.AspNetCore.Http;

namespace HUBT_Social_Base.Service
{
    public interface IHttpCloudService : IBaseService
    {
        Task<string?> GetUrlFormBase6(UploadBase64Request request);
        Task<string?> GetUrlFormFile(FileRequest request);

    }
}