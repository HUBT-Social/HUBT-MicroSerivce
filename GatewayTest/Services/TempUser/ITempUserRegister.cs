using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.Requests;

namespace GatewayTest.Services.TempUser
{
    public interface ITempUserRegister : IBaseService
    {
        Task<HttpResponseMessage> Ping();
    }
}
