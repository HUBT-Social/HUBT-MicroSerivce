using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.Requests.Firebase;

namespace TempRegister_API.Src.Service
{
    public class NotifiService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), INotifiService
    {
        public async Task SendRemindNotication(List<NotificatonRemindRequest> remindNotication)
        {
            string path = "api/notification/send-remind-to-multi-username";
            await SendRequestAsync(path,HUBT_Social_Core.Settings.@enum.ApiType.POST, remindNotication);
        }
    }
}
