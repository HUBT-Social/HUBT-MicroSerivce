using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.ExamDTO;

namespace User_API.Src.Service
{
    public interface IHelperService : IBaseService
    {
        public Task<List<Question>> ExtractQuestions(IFormFile file);

    }
    
}
