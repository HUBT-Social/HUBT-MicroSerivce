using HUBT_Social_Core.Models.Requests.Firebase;
using MongoDB.Driver;
using Notation_API.Src.Controllers;

namespace Notation_API.Src.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IMongoCollection<ScheduledNotification> _scheduledNotifications;
        private readonly IMongoCollection<NotificationHistory> _notificationHistory;

        public NotificationRepository(IMongoDatabase database)
        {
            _scheduledNotifications = database.GetCollection<ScheduledNotification>("ScheduledNotifications");
            _notificationHistory = database.GetCollection<NotificationHistory>("NotificationHistory");
        }

        public async Task SaveScheduledNotificationAsync(ScheduledNotification notification)
        {
            await _scheduledNotifications.InsertOneAsync(notification);
        }

        public async Task<ScheduledNotification?> GetScheduledNotificationAsync(string id)
        {
            return await _scheduledNotifications.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateScheduledNotificationAsync(ScheduledNotification notification)
        {
            await _scheduledNotifications.ReplaceOneAsync(x => x.Id == notification.Id, notification);
        }

        public async Task UpdateScheduledNotificationStatusAsync(string id, string status)
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

        public async Task<PagedResult<ScheduledNotification>> GetScheduledNotificationsAsync(int page, int pageSize, string? status = null, string? createdBy = null)
        {
            var filterBuilder = Builders<ScheduledNotification>.Filter;
            var filters = new List<FilterDefinition<ScheduledNotification>>();

            if (!string.IsNullOrEmpty(status))
            {
                filters.Add(filterBuilder.Eq(x => x.Status, status));
            }

            if (!string.IsNullOrEmpty(createdBy))
            {
                filters.Add(filterBuilder.Eq(x => x.CreatedBy, createdBy));
            }

            var filter = filters.Count > 0
                ? filterBuilder.And(filters)
                : filterBuilder.Empty;

            var total = await _scheduledNotifications.CountDocumentsAsync(filter);
            var items = await _scheduledNotifications
                .Find(filter)
                .Sort(Builders<ScheduledNotification>.Sort.Descending(x => x.CreatedAt))
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

        public async Task SaveHistoryAsync(NotificationHistory history)
        {
            await _notificationHistory.InsertOneAsync(history);
        }

        public async Task<List<NotificationHistory>> GetNotificationHistoryAsync(int page, int pageSize, string? type = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var filterBuilder = Builders<NotificationHistory>.Filter;
            var filters = new List<FilterDefinition<NotificationHistory>>();

            if (!string.IsNullOrEmpty(type))
            {
                filters.Add(filterBuilder.Eq(x => x.Type, type));
            }

            if (fromDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(x => x.CreatedAt, fromDate.Value));
            }

            if (toDate.HasValue)
            {
                filters.Add(filterBuilder.Lte(x => x.CreatedAt, toDate.Value));
            }

            var filter = filters.Count > 0
                ? filterBuilder.And(filters)
                : filterBuilder.Empty;

            var items = await _notificationHistory
                .Find(filter)
                .Sort(Builders<NotificationHistory>.Sort.Descending(x => x.CreatedAt))
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return items;
        }
    }
}
