using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests;

namespace Auth_API.Src.Services.TempUser
{
    public interface ITempUserRegister : IBaseService
    {
        Task<ResponseDTO> StoreIn(RegisterRequest request);
        Task<ResponseDTO> Get(string email);
    }
}
