using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings.@enum;

namespace User_API.Src.Service
{
    public interface ITempService : IBaseService
    {
        Task<TimetableOutputDTO> StoreIn(TimetableOutputDTO request);
        Task<TimetableOutputDTO> Get(string id);
        Task<List<TimetableOutputDTO>> GetList(string className);
        Task<ClassScheduleVersionDTO> GetClassScheduleVersion(string className);
        
        Task<ClassScheduleVersionDTO> StoreClassScheduleVersion(string className,DateTime expireTime);
        Task<ClassScheduleVersionDTO> StoreClassScheduleVersion(ClassScheduleVersionDTO request);


    }
}
