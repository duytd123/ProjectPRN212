using BusinessObjects;
using DataAccess.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjectPRN212
{
    public partial class NotificationWindow : Window
    {
        private readonly NotifyObject _notifyObject;
        private readonly ReportObjects _reportObjects;
        private readonly int _userId;
        private List<Notification> _notifications;

        public NotificationWindow(int userId, NotifyObject notifyObject)
        {
            InitializeComponent();
            _userId = userId;
            _notifyObject = notifyObject;
            LoadNotifications();
            NotificationsListBox.MouseDoubleClick += NotificationsListBox_MouseDoubleClick;
        }

        private void LoadNotifications()
        {
            try
            {
                _notifications = _notifyObject.GetUserNotifications(_userId);
                NotificationsListBox.ItemsSource = _notifications;
                NotificationsListBox.Loaded += NotificationsListBox_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông báo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NotificationsListBox_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in NotificationsListBox.Items)
            {
                var listBoxItem = NotificationsListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                if (listBoxItem != null)
                {
                    var border = FindVisualChild<Border>(listBoxItem);
                    var textBlock = FindVisualChild<TextBlock>(listBoxItem, "NotificationMessage");
                    if (border != null && textBlock != null && item is Notification notification)
                    {
                        if (!notification.IsRead)
                        {
                            textBlock.FontWeight = FontWeights.Bold;
                            border.Background = Brushes.LightYellow;
                        }
                        else
                        {
                            textBlock.FontWeight = FontWeights.Normal;
                            border.Background = Brushes.White;
                        }
                    }
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject parent, string childName = null) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result && (childName == null || (child is FrameworkElement element && element.Name == childName)))
                {
                    return result;
                }
                var descendant = FindVisualChild<T>(child, childName);
                if (descendant != null)
                {
                    return descendant;
                }
            }
            return null;
        }

        private void NotificationsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedNotification = NotificationsListBox.SelectedItem as Notification;
            if (selectedNotification != null)
            {
                _notifyObject.MarkNotificationAsRead(selectedNotification.NotificationId);
                LoadNotifications();
            }
        }

        private void MarkAllAsReadButton_Click(object sender, RoutedEventArgs e)
        {
            var notifications = _notifyObject.GetUserNotifications(_userId);
            foreach (var notification in notifications)
            {
                _notifyObject.MarkNotificationAsRead(notification.NotificationId);
            }
            LoadNotifications();
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedNotification = NotificationsListBox.SelectedItem as Notification;
            if (selectedNotification != null)
            {
                _notifyObject.DeleteNotification(selectedNotification.NotificationId);
                LoadNotifications();
            }
        }
    }
}
