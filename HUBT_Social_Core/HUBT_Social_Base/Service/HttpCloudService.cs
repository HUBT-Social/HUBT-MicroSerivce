using Amazon.Runtime.Internal;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Models;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests.Cloud;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Base.Service
{
    public class HttpCloudService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IHttpCloudService
    {
        public async Task<string?> GetUrlFormBase6(UploadBase64Request request)
        {
            string path = $"api/cloudinary/upload-base64-file";
            ResponseDTO responseDTO = await SendRequestAsync(path, ApiType.POST, request, null);
            if (responseDTO.StatusCode == System.Net.HttpStatusCode.OK)
            {
                FileUploadResult? response = responseDTO.ConvertTo<FileUploadResult>();
                if (response == null)
                {
                    return null;
                }
                return response.Url;
            }
            return null;
        }

        public async Task<string?> GetUrlFormFile(FileRequest request)
        {
            if (request.file == null) return null;

            using var memoryStream = new MemoryStream();
            await request.file.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            string base64Data = Convert.ToBase64String(fileBytes);
            string contentType = request.file.ContentType;

            string base64StringWithPrefix = $"data:{contentType};base64,{base64Data}";

            UploadBase64Request base64Resquest = new()
            {
                Base64String = base64StringWithPrefix,
                FileName = request.file.FileName
            };

           return await GetUrlFormBase6(base64Resquest);
        }
    }
}
