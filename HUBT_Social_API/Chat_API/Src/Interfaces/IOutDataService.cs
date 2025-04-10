using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;

namespace Chat_API.Src.Interfaces
{
    public interface IOutDataService : IBaseService
    {
       Task<ResponseDTO> GetStudentsByClassName(string className);
    }
}
