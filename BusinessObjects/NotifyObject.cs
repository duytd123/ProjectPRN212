using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects
{
    public class NotifyObject
    {
        private readonly NotifyRepository _notificationRepository;
        private readonly ProjectPrn212Context _context;

        public NotifyObject()
        {
            _context = new ProjectPrn212Context();
            _notificationRepository = new NotifyRepository(new ProjectPrn212Context());
        }

        public NotifyObject(NotifyRepository notificationRepository)
        {
            _context = new ProjectPrn212Context();
            _notificationRepository = notificationRepository;
        }

        public List<Notification> GetUserNotifications(int userId)
        {
            return _notificationRepository.GetNotificationsByUserId(userId);
        }

        public void MarkNotificationAsRead(int notificationId)
        {
            _notificationRepository.MarkAsRead(notificationId);
        }

        public void DeleteNotification(int notificationId)
        {
            _notificationRepository.DeleteNotification(notificationId);
        }

        public void AddNotification(int userId, string message, string plateNumber = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                PlateNumber = plateNumber,
                SentDate = DateTime.Now,
                IsRead = false
            };

            _notificationRepository.AddNotification(notification);
        }
    }
}
