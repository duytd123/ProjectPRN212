using DataAccess.Models;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    public class NotifyRepository : INotifyRepository
    {
        private readonly ProjectPrn212Context _context;

        public NotifyRepository(ProjectPrn212Context context)
        {
            _context = context;
        }

        public List<Notification> GetNotificationsByUserId(int userId)
        {
            try
            {
                var context = new ProjectPrn212Context();
                return context.Notifications
                    .Include(n => n.PlateNumberNavigation)
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.SentDate)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public List<Notification> GetUnreadNotificationsByUserId(int userId)
        {
            try
            {
                var context = new ProjectPrn212Context();
                return context.Notifications
                    .Include(n => n.PlateNumberNavigation)
                    .Where(n => n.UserId == userId && (n.IsRead == false || n.IsRead == null))
                    .OrderByDescending(n => n.SentDate)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public bool MarkAsRead(int notificationId)
        {
            try
            {
                var context = new ProjectPrn212Context();
                var notification = context.Notifications.Find(notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
