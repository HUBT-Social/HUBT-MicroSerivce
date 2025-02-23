using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;

namespace TestLibary.Service
{
    public interface INotationService : IBaseService
    {
        Task<StudentDTO?> GetStudentByMasv(string masv);
        Task<List<TimeTableDTO>?> GetTimeTableByClassName(string className);
        Task<AVGScoreDTO?> GetAVGScoreByMasv(string masv);
        Task<List<ScoreDTO>?> GetStudentScoreByMasv(string masv);


    }
}
