using DataAccess.Models;
using DataAccess.Repository.Interface;

namespace DataAccess.Repository
{
    public class NotifyRepository : INotifyRepository
    {
        private readonly ProjectPrn212Context _context;

        public NotifyRepository(ProjectPrn212Context context)
        {
            _context = context;
        }

        public void DeleteNotification(int notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                _context.SaveChanges();
            }
        }

        public void AddNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public List<Notification> GetNotificationsByUserId(int userId)
        {
            return _context.Notifications.Where(n => n.UserId == userId).ToList();
        }

        public Notification GetNotificationById(int notificationId)
        {
            return _context.Notifications.FirstOrDefault(n => n.NotificationId == notificationId);
        }

        public void MarkAsRead(int notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }
        }
    }
}
