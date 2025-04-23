using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests;

namespace Chat_API.Src.Interfaces
{
    public interface ICourseService : IBaseService
    {
        Task<List<CreateGroupByCourse>> GetCourse(int page);
    }
}