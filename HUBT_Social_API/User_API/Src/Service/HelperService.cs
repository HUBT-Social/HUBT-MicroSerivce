using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs.ExamDTO;
using HUBT_Social_Core.Settings.@enum;
using System.Net.Http.Headers;

namespace User_API.Src.Service
{
    public class HelperService(
        IHttpService httpService, 
        string basePath) : BaseService(httpService,basePath),IHelperService 
    {
        public async Task<List<Question>> ExtractQuestions(IFormFile file)
        {
            using MultipartFormDataContent content = new();

            using Stream stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            // Set chuẩn cả content-disposition để tránh lỗi 422 từ FastAPI
            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = $"\"{file.FileName}\""
            };

            content.Add(streamContent);

            HttpResponseMessage result = await SendActionResultRequestAsync("extract-questions", ApiType.POST, content);
            List<Question> questions = await result.ConvertTo<List<Question>>() ?? [];
            return questions;
        }
    }
}
