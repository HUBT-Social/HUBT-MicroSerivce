using HUBT_Social_Base.Service;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using HUBT_Social_Core.Models.Requests.Firebase;

namespace Chat_Data_API.Src.Service
{
    public class Notition(IHttpService httpService, string basePath) : BaseService(httpService, basePath), INotition
    {
        public async Task SendNotationToMany(SendGroupMessageRequest request, string accessToken)
        {
            try
            {
                string path = $"api/notation/send-to-many";
                await SendActionResultRequestAsync(path, ApiType.POST, request, accessToken);
            }
            catch { }
        }

        public async Task SendNotationToOne(SendMessageRequest request,string accessToken)
        {
            try
            {
                string path = $"api/notation/send-to-one";
                await SendActionResultRequestAsync(path, ApiType.POST, request, accessToken);
            }
            catch { }
        }
    }
}
