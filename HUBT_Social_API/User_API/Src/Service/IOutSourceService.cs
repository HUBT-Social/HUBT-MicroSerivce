using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;

namespace User_API.Src.Service
{
    public interface IOutSourceService : IBaseService
    {
        Task<StudentDTO?> GetStudentByMasv(string masv);
        Task<List<TimeTableDTO>?> GetTimeTableByClassName(string className);
        Task<TimeTableDTO?> GetTimeTableById(string id);
        Task<AVGScoreDTO?> GetAVGScoreByMasv(string masv);
        Task<List<ScoreDTO>?> GetStudentScoreByMasv(string masv);
        Task<List<SubjectDTO>?> GetCouresAsync(string className);
        Task<List<StudentDTO>> GetStudentByClassName(string className);
        Task<List<StudentClassName>> GetSlideStudentClassName(int page);
    }
}
