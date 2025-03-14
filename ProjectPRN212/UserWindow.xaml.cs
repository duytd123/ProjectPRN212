using BusinessObjects;
using DataAccess.Models;
using System.Windows;

namespace ProjectPRN212
{
    public partial class UserWindow : Window
    {
        private readonly int currentUserId;
        private readonly UserObject _userObjects;
        private readonly ReportObjects _reportObjects;
        private User _currentUser;

        public UserWindow(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            _userObjects = new UserObject();
            _reportObjects = new ReportObjects();
            LoadCurrentUser();
        }

        private void SendFeedback_Click(object sender, RoutedEventArgs e)
        {
            var sendFeedback = new SendFeedback(currentUserId);
            sendFeedback.ShowDialog();
        }

        private void LoadCurrentUser()
        {
            try
            {
                // Fetch user data using UserObject
                _currentUser = _userObjects.GetUserById(currentUserId);

                if (_currentUser == null)
                {
                    MessageBox.Show("Không thể tải thông tin người dùng!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void TrackStatus_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null)
            {
                var trackingWindow = new TrackStatusWindow(_currentUser);
                trackingWindow.Show();
            }
            else
            {
                MessageBox.Show("Không thể tải thông tin người dùng!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewViolationDetails_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PayFineOnline_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
