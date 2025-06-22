using HUBT_Social_Base;
using HUBT_Social_Core.Models.Requests.Firebase;

namespace TempRegister_API.Src.Service
{
    public interface INotifiService : IBaseService
    {
        Task SendRemindNotication(List<NotificatonRemindRequest> remindNotication);
    }
}
