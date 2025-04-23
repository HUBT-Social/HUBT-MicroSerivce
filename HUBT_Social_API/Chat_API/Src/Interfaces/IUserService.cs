using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;

namespace Chat_API.Src.Interfaces
{
    public interface IUserService : IBaseService
    {
        Task<ResponseDTO> GetUserRequest(string accessToken);
        Task<List<AUserDTO>?> GetUsersByUserNames(ListUserNameDTO request, string accessToken);
        Task<AUserDTO?> GetUserByUserName(string userName, string accessToken);
        Task<ResponseDTO> GetAllUser(string accessToken);
    }
}
