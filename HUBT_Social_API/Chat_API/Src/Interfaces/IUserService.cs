using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;

namespace Chat_API.Src.Interfaces
{
    public interface IUserService : IBaseService
    {
        Task<HttpResponseMessage> GetUserRequest(string accessToken);
        Task<AUserDTO?> GetUserById(string? userId, string accessToken);
        Task<HttpResponseMessage> GetAllUser(string accessToken);
    }
}
