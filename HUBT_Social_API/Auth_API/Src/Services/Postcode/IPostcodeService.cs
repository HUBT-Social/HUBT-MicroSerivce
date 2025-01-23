using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Models.Requests;

namespace Auth_API.Src.Services.Postcode
{
    public interface IPostcodeService : IBaseService
    {
        Task<PostCodeDTO?> GetCurrentPostCode(PostcodeRequest request);

        Task<ResponseDTO> SendVerificationEmail(string email, string userName, string userAgent, string ipAddress);
    }
}
