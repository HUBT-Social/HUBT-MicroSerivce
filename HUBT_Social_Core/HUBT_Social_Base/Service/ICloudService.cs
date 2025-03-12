using HUBT_Social_Base.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Base.Service
{
    public interface ICloudService
    {
        Task<List<FileUploadResult>> UploadFilesAsync(List<IFormFile> files);
        Task<FileUploadResult?> UploadFileAsync(IFormFile file);
        Task<string?> UpdateAvatarAsync(string filePath, IFormFile file);
    }
}
