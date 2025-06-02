//using Notation_API.Src.Controllers;

//namespace Notation_API.Src.Repository
//{
//    public interface INotificationRepository
//    {
//        Task SaveScheduledNotificationAsync(NotationController.ScheduledNotification notification);
//        Task<NotationController.ScheduledNotification?> GetScheduledNotificationAsync(string id);
//        Task UpdateScheduledNotificationAsync(NotationController.ScheduledNotification notification);
//        Task UpdateScheduledNotificationStatusAsync(string id, string status);
//        Task<PagedResult<NotationController.ScheduledNotification>> GetScheduledNotificationsAsync(int page, int pageSize, string? status = null);
//        Task SaveHistoryAsync(NotationController.NotificationHistory history);
//    }

//    public class PagedResult<T>
//    {
//        public List<T> Items { get; set; } = new();
//        public int TotalCount { get; set; }
//        public int PageNumber { get; set; }
//        public int PageSize { get; set; }
//        public int TotalPages { get; set; }
//        public bool HasPreviousPage => PageNumber > 1;
//        public bool HasNextPage => PageNumber < TotalPages;
//    }
//}
