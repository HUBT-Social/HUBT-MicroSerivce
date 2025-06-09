using HUBT_Social_Core.Models.Requests.Firebase;
using MongoDB.Driver;
using System.Xml.Schema;

namespace Notation_API.Src.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IMongoCollection<ScheduledNotification> _scheduledNotifications;
        private readonly IMongoCollection<NotificationHistory> _notificationHistory;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(IMongoDatabase database, ILogger<NotificationRepository> logger)
        {
            _scheduledNotifications = database.GetCollection<ScheduledNotification>("ScheduledNotifications");
            _notificationHistory = database.GetCollection<NotificationHistory>("NotificationHistory");
            _logger = logger;
        }

        public async Task SaveScheduledNotificationAsync(ScheduledNotification notification)
        {
            try
            {
                await _scheduledNotifications.InsertOneAsync(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu thông báo lên lịch.");
            }
        }

        public async Task<ScheduledNotification?> GetScheduledNotificationAsync(string id)
        {
            try
            {
                return await _scheduledNotifications.Find(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy thông báo với ID: {id}");
                return null;
            }
        }

        public async Task UpdateScheduledNotificationAsync(ScheduledNotification notification)
        {
            try
            {
                await _scheduledNotifications.ReplaceOneAsync(x => x.Id == notification.Id, notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật thông báo với ID: {notification.Id}");
            }
        }

        public async Task UpdateScheduledNotificationStatusAsync(string id, string status)
        {
            try
            {
                var update = Builders<ScheduledNotification>.Update
                    .Set(x => x.Status, status)
                    .Set(x => x.ProcessedAt, DateTime.UtcNow);

                if (status == "Failed")
                {
                    update = update.Inc(x => x.RetryCount, 1);
                }

                await _scheduledNotifications.UpdateOneAsync(x => x.Id == id, update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật trạng thái thông báo với ID: {id}");
            }
        }

        public async Task<PagedResult<ScheduledNotification>> GetScheduledNotificationsAsync(int page, int pageSize, string? status = null, string? createdBy = null)
        {
            try
            {
                var filterBuilder = Builders<ScheduledNotification>.Filter;
                var filters = new List<FilterDefinition<ScheduledNotification>>();

                if (!string.IsNullOrEmpty(status))
                    filters.Add(filterBuilder.Eq(x => x.Status, status));

                if (!string.IsNullOrEmpty(createdBy))
                    filters.Add(filterBuilder.Eq(x => x.CreatedBy, createdBy));

                var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

                var total = await _scheduledNotifications.CountDocumentsAsync(filter);
                var items = await _scheduledNotifications
                    .Find(filter)
                    .SortByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Limit(pageSize)
                    .ToListAsync();

                return new PagedResult<ScheduledNotification>
                {
                    Items = items,
                    TotalCount = (int)total,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thông báo có phân trang.");
                return new PagedResult<ScheduledNotification>
                {
                    Items = new List<ScheduledNotification>(),
                    TotalCount = 0,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = 0
                };
            }
        }

        public async Task SaveHistoryAsync(NotificationHistory history)
        {
            try
            {
                await _notificationHistory.InsertOneAsync(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu lịch sử thông báo.");
            }
        }

        public async Task<List<NotificationHistory>> GetNotificationHistoryAsync(int startAt, int pageSize)
        {
            try
            {
                int sizeHistory = (int)await _notificationHistory.CountDocumentsAsync(FilterDefinition<NotificationHistory>.Empty);
                if (sizeHistory <= startAt) 
                {
                    return [];
                }
                int skip = Math.Max(0, sizeHistory - startAt - pageSize);
                if (skip == 0)
                {
                    pageSize = sizeHistory - startAt;
                }

                var items = await _notificationHistory
                    .Find(FilterDefinition<NotificationHistory>.Empty)
                    .SortByDescending(x => x.CreatedAt)
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử thông báo.");
                return new List<NotificationHistory>();
            }
        }

        public async Task<bool> DeleteNotificationByIdAsync(string id)
        {
            try
            {
                var result = await _notificationHistory.DeleteOneAsync(x => x.Id == id);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa thông báo với ID: {id}");
                return false;
            }
        }
    }

}
