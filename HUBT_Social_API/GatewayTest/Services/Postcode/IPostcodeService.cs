using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Models.Requests;

namespace GatewayTest.Services.Postcode
{
    public interface IPostcodeService : IBaseService
    {
        Task<HttpResponseMessage> Ping();
    }
}
