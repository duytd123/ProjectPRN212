using BusinessObjects;
using DataAccess.Models;
using System.Windows;

namespace ProjectPRN212
{
    public partial class NotificationWindow : Window
    {
        private List<Notification> _notifications;
        private NotifyObject _notifyObject;

        public NotificationWindow(List<Notification> notifications)
        {
            InitializeComponent();
            _notifications = notifications;
            _notifyObject = new NotifyObject();
            UpdateNotificationList();
        }

        private void UpdateNotificationList()
        {
            NotificationList.ItemsSource = null;
            NotificationList.ItemsSource = _notifications;
        }

        private void MarkAllAsReadButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ProjectPrn212Context())
            {
                foreach (var notification in _notifications)
                {
                    var dbNotification = context.Notifications.Find(notification.NotificationId);
                    if (dbNotification != null)
                    {
                        dbNotification.IsRead = true;
                    }
                }
                context.SaveChanges();
            }
            foreach (var notification in _notifications)
            {
                notification.IsRead = true;
            }
            UpdateNotificationList();
            MessageBox.Show("Tất cả thông báo đã được đánh dấu là đã đọc.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void NotificationList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedNotification = NotificationList.SelectedItem as Notification;
            if (selectedNotification == null) return;

            if (selectedNotification.IsRead != true)
            {
                using (var context = new ProjectPrn212Context())
                {
                    var dbNotification = context.Notifications.Find(selectedNotification.NotificationId);
                    if (dbNotification != null)
                    {
                        dbNotification.IsRead = true;
                        context.SaveChanges();
                    }
                }
                selectedNotification.IsRead = true;
            }
            OpenDetailWindow(selectedNotification);
            UpdateNotificationList();
        }

        private void OpenDetailWindow(Notification notification)
        {
            NotificationDetailWindow detailWindow = new NotificationDetailWindow(notification);
            detailWindow.Owner = this;
            detailWindow.ShowDialog();
        }
    }
}
