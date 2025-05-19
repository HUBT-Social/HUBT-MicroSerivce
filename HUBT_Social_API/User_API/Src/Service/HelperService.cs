using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs.ExamDTO;
using HUBT_Social_Core.Settings.@enum;

namespace User_API.Src.Service
{
    public class HelperService(
        IHttpService httpService, 
        string basePath) : BaseService(httpService,basePath),IHelperService 
    {
        public async Task<List<Question>> ExtractQuestions(IFormFile file)
        {
            HttpResponseMessage result = await SendActionResultRequestAsync("extract-questions", ApiType.POST, file);
            List<Question> questions = await result.ConvertTo<List<Question>>() ?? [];
            return questions;
        }
    }
}
