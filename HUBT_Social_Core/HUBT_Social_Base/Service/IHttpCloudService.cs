
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests.Cloud;

namespace HUBT_Social_Base.Service
{
    public interface IHttpCloudService : IBaseService
    {
        Task<string?> GetUrlFormFile(UploadBase64Request request);

    }
}