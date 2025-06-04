using HUBT_Social_Core.Models.Requests.Firebase;
using Notation_API.Src.Controllers;

namespace Notation_API.Src.Repository
{
    public interface INotificationRepository
    {
        Task SaveScheduledNotificationAsync(ScheduledNotification notification);
        Task<ScheduledNotification?> GetScheduledNotificationAsync(string id);
        Task UpdateScheduledNotificationAsync(ScheduledNotification notification);
        Task UpdateScheduledNotificationStatusAsync(string id, string status);
        Task<PagedResult<ScheduledNotification>> GetScheduledNotificationsAsync(int page, int pageSize, string? status = null, string? createdBy = null);
        Task SaveHistoryAsync(NotificationHistory history);
        Task<List<NotificationHistory>> GetNotificationHistoryAsync(int page, int pageSize, string? type = null, DateTime? fromDate = null, DateTime? toDate = null);
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
