//using MongoDB.Driver;
//using Notation_API.Src.Controllers;

//namespace Notation_API.Src.Repository
//{
//    public class NotificationRepository : INotificationRepository
//    {
//        private readonly IMongoCollection<NotationController.ScheduledNotification> _scheduledNotifications;
//        private readonly IMongoCollection<NotationController.NotificationHistory> _notificationHistory;

//        public NotificationRepository(IMongoDatabase database)
//        {
//            _scheduledNotifications = database.GetCollection<NotationController.ScheduledNotification>("ScheduledNotifications");
//            _notificationHistory = database.GetCollection<NotationController.NotificationHistory>("NotificationHistory");
//        }

//        public async Task SaveScheduledNotificationAsync(NotationController.ScheduledNotification notification)
//        {
//            await _scheduledNotifications.InsertOneAsync(notification);
//        }

//        public async Task<NotationController.ScheduledNotification?> GetScheduledNotificationAsync(string id)
//        {
//            return await _scheduledNotifications.Find(x => x.Id == id).FirstOrDefaultAsync();
//        }

//        public async Task UpdateScheduledNotificationAsync(NotationController.ScheduledNotification notification)
//        {
//            await _scheduledNotifications.ReplaceOneAsync(x => x.Id == notification.Id, notification);
//        }

//        public async Task UpdateScheduledNotificationStatusAsync(string id, string status)
//        {
//            var update = Builders<NotationController.ScheduledNotification>.Update
//                .Set(x => x.Status, status)
//                .Set(x => x.ProcessedAt, DateTime.UtcNow);

//            if (status == "Failed")
//            {
//                update = update.Inc(x => x.RetryCount, 1);
//            }

//            await _scheduledNotifications.UpdateOneAsync(x => x.Id == id, update);
//        }

//        public async Task<PagedResult<NotationController.ScheduledNotification>> GetScheduledNotificationsAsync(int page, int pageSize, string? status = null)
//        {
//            var filter = status != null
//                ? Builders<NotationController.ScheduledNotification>.Filter.Eq(x => x.Status, status)
//                : Builders<NotationController.ScheduledNotification>.Filter.Empty;

//            var total = await _scheduledNotifications.CountDocumentsAsync(filter);

//            var items = await _scheduledNotifications
//                .Find(filter)
//                .Sort(Builders<NotationController.ScheduledNotification>.Sort.Descending(x => x.CreatedAt))
//                .Skip((page - 1) * pageSize)
//                .Limit(pageSize)
//                .ToListAsync();

//            return new PagedResult<NotationController.ScheduledNotification>
//            {
//                Items = items,
//                TotalCount = (int)total,
//                PageNumber = page,
//                PageSize = pageSize,
//                TotalPages = (int)Math.Ceiling((double)total / pageSize)
//            };
//        }

//        public async Task SaveHistoryAsync(NotationController.NotificationHistory history)
//        {
//            await _notificationHistory.InsertOneAsync(history);
//        }
//    }
//}
