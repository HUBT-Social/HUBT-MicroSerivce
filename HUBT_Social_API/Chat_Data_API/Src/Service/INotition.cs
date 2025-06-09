using HUBT_Social_Base;
using HUBT_Social_Core.Models.Requests.Firebase;

namespace Chat_Data_API.Src.Service
{
    public interface INotition : IBaseService
    {
        Task SendNotationToOne(SendNotificationToOneUserNameRequest request, string accessToken);
        Task SendNotationToMany(SendNotificationToMultiUserNamesRequest request, string accessToken);
        Task SendNotationToGroupChat(SendNotificationToMultiUserNamesRequest request, string accessToken);
    }
}
