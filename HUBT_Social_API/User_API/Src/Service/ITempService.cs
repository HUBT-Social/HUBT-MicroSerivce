using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.Requests;

namespace User_API.Src.Service
{
    public interface ITempService : IBaseService
    {
        Task<bool> StoreIn(TimetableOutputDTO request);
        Task<TimetableOutputDTO> Get(string id);
    }
}
