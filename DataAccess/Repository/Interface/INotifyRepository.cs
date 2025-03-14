using DataAccess.Models;

namespace DataAccess.Repository.Interface
{
    public interface INotifyRepository
    {
        public List<Notification> GetNotificationsByUserId(int userId);
        public List<Notification> GetUnreadNotificationsByUserId(int userId);
        public bool MarkAsRead(int notificationId);
    }
}
