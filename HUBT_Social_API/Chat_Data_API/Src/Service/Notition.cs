using HUBT_Social_Base.Service;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using HUBT_Social_Core.Models.Requests.Firebase;

namespace Chat_Data_API.Src.Service
{
    public class Notition(IHttpService httpService, string basePath) : BaseService(httpService, basePath), INotition
    {
        public async Task SendNotationToGroupChat(SendNotificationToMultiUserNamesRequest request, string accessToken)
        {
            try
            {
                string path = $"api/notification/send-to-multi-username";
                await SendActionResultRequestAsync(path, ApiType.POST, request, accessToken);
            }
            catch { }
        }

        public async Task SendNotationToMany(SendNotificationToMultiUserNamesRequest request, string accessToken)
        {
            try
            {
                string path = $"api/notification/send-to-multi-username";
                await SendActionResultRequestAsync(path, ApiType.POST, request, accessToken);
            }
            catch { }
        }

        public async Task SendNotationToOne(SendNotificationToOneUserNameRequest request, string accessToken)
        {
            try
            {
                string path = $"api/notification/send-to-one-username";
                await SendActionResultRequestAsync(path, ApiType.POST, request, accessToken);
            }
            catch { }
        }
    }
}
