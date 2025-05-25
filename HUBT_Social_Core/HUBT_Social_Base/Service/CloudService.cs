using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HUBT_Social_Base.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Base.Service
{
     public class CloudService : ICloudService
     {
        private readonly Cloudinary _cloudinary;

        public CloudService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
        }

        public async Task<List<FileUploadResult>> UploadFilesAsync(List<IFormFile> files)
        {
            var results = new List<FileUploadResult>();
            foreach (var file in files)
            {
                var result = await UploadFileAsync(file);
                if (result != null) results.Add(result);
            }
            return results;
        }

        public async Task<FileUploadResult?> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length <= 0) return null;

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult?.StatusCode == HttpStatusCode.OK)
                {
                    return new FileUploadResult
                    {
                        Url = uploadResult.Url?.ToString(),
                        ResourceType = uploadResult.ResourceType
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cloudinary upload error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ReplaceImageWithAlreadyPathAsync(string filePath, IFormFile file)
        {
            if (file == null || file.Length <= 0) return null;

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = filePath,
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult?.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cloudinary avatar update error: {ex.Message}");
                return null;
            }
        }
    }
}

