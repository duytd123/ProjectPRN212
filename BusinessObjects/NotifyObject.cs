using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects
{
    public class NotifyObject
    {
        private readonly INotifyRepository _notifyRepository;
        private readonly ProjectPrn212Context _context;

        public NotifyObject()
        {
            _context = new ProjectPrn212Context();
            _notifyRepository = new NotifyRepository(_context);
        }
        public List<Notification> GetNotificationsByUserId(int userId)
        {
            var notify = _notifyRepository.GetNotificationsByUserId(userId);
            return notify;
        }
        public List<Notification> GetUnreadNotificationsByUserId(int userId)
        {
            var notify = _notifyRepository.GetUnreadNotificationsByUserId(userId);
            return notify;
        }
        public bool MarkAsRead(int notificationId)
        {
            var notify = _notifyRepository.MarkAsRead(notificationId);
            return notify;
        }
    }
}
