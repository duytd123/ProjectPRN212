using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.Interface
{
    public interface INotifyRepository
    {
        void AddNotification(Notification notification);
        List<Notification> GetNotificationsByUserId(int userId);
        void MarkAsRead(int notificationId);
        void DeleteNotification(int notificationId);
        Notification GetNotificationById(int notificationId);
    }
}
