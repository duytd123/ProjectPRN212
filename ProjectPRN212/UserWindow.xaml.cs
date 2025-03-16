using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using DataAccess.Models;
using System.Windows;

namespace ProjectPRN212
{
    public partial class UserWindow : Window
    {

        private DispatcherTimer _sessionTimer;
        private readonly AdminObject _adminObject;
        public UserWindow()
        {
            InitializeComponent();
            _adminObject = new AdminObject(new DataAccess.Repository.AdminRepository(new DataAccess.Models.ProjectPrn212Context()));
            if (_adminObject.IsAutoLogoutEnabled())
            {
                int timeoutMinutes = _adminObject.GetSessionTimeout();
                StartSessionTimer(timeoutMinutes);
            }
        }

        private void StartSessionTimer(int timeoutMinutes)
        {
            _sessionTimer = new DispatcherTimer();
            _sessionTimer.Interval = TimeSpan.FromMinutes(timeoutMinutes);
            _sessionTimer.Tick += SessionTimeoutHandler;
            _sessionTimer.Start();

            this.MouseMove += ResetSessionTimer;
            this.KeyDown += ResetSessionTimer;
        }

        private void ResetSessionTimer(object sender, EventArgs e)
        {
            _sessionTimer.Stop();
            _sessionTimer.Start();
        }

        private void SessionTimeoutHandler(object sender, EventArgs e)
        {
            _sessionTimer?.Stop();
            MessageBox.Show("Bạn đã bị đăng xuất tự động do không hoạt động.", "Hết thời gian phiên", MessageBoxButton.OK, MessageBoxImage.Information);
            if (!IsLoginPageOpen())
            {
                LoginPage loginPage = new LoginPage();
                loginPage.Show();
            }
            this.Close();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            _sessionTimer?.Stop();
            MessageBox.Show("Bạn đã đăng xuất thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            this.Close();
        }

        private bool IsLoginPageOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginPage)
                {
                    return true; 
                }
            }
            return false;
            
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
