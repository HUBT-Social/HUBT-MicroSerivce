using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;

namespace TempRegister_API.Src.Service
{
    public interface IOutService : IBaseService
    {
        Task<ResponseDTO> GetUserInClass(string className);
        Task<ResponseDTO> GetCoureInfo(string courseName);
    }
}
