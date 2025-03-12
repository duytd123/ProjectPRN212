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

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
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
        }
    }
}
