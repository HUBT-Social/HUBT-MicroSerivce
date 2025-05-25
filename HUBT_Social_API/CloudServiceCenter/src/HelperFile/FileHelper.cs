
using HUBT_Social_Core.Models.Requests.Cloud;
using System.Text.RegularExpressions;

namespace CloudServiceCenter.src.HelperFile
{
    public static class FileHelper
    {
        public static IFormFile? Base64ToFormFile(string base64String, string fileName)
        {
            if (string.IsNullOrWhiteSpace(base64String) || string.IsNullOrWhiteSpace(fileName))
                return null;

            string? base64Data;

            // Trường hợp có prefix như: "data:image/png;base64,..."
            var match = Regex.Match(base64String, @"data:(?<type>.+?);base64,(?<data>.+)");
            if (match.Success)
            {
                base64Data = match.Groups["data"].Value;
            }
            else
            {
                // Trường hợp chỉ có chuỗi base64 thuần
                base64Data = base64String;
            }

            try
            {
                byte[] bytes = Convert.FromBase64String(base64Data);
                var stream = new MemoryStream(bytes);
                var contentType = GetContentTypeFromFileName(fileName);

                return new FormFile(stream, 0, bytes.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = contentType
                };
            }
            catch (FormatException)
            {
                // Không phải base64 hợp lệ
                return null;
            }
            catch (Exception)
            {
                // Lỗi khác trong quá trình xử lý
                return null;
            }
        }
        public static async Task<Base64FileResult?> FormFileToBase64ObjectAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            string base64Data = Convert.ToBase64String(fileBytes);
            string contentType = file.ContentType;

            string base64StringWithPrefix = $"data:{contentType};base64,{base64Data}";

            return new Base64FileResult
            {
                Base64String = base64StringWithPrefix,
                FileName = file.FileName
            };
        }


        public static string GetContentTypeFromFileName(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".pdf" => "application/pdf",
                ".mp4" => "video/mp4",
                _ => "application/octet-stream"
            };
        }
    }
}
