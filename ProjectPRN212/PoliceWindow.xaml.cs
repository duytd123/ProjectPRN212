using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Windows;
using DataAccess.Models;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN212
{
    public partial class PoliceWindow : Window
    {
        private PoliceObject _policeObject;
        private List<Report> AllReports;
        private DispatcherTimer _sessionTimer;
        private readonly AdminObject _adminObject;
        private int _policeUserId;
        public PoliceWindow(PoliceObject policeObject, int policeUserId)
        {
            InitializeComponent();
            _policeObject = policeObject;
            _policeUserId = policeUserId;
            LoadReports();

            _adminObject = new AdminObject(new DataAccess.Repository.AdminRepository(new DataAccess.Models.ProjectPrn212Context()));
            if (_adminObject.IsAutoLogoutEnabled())
            {
                int timeoutMinutes = _adminObject.GetSessionTimeout();
                StartSessionTimer(timeoutMinutes);
            }
        }

        private void LoadReports()
        {
            try
            {
                AllReports = _policeObject.GetAllReports()
            .Include(r => r.Reporter)
            .Include(r => r.ProcessedByNavigation)
            .Include(r => r.Violations)
            .Include(r => r.ViolationType)
            .ToList();

                foreach (var report in AllReports)
                {
                    report.NotificationSent = _policeObject.HasNotificationBeenSent(report.ReportId);
                }

                ReportsDataGrid.ItemsSource = AllReports;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách biên bản:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.SelectedItem is Report selectedReport)
            {
                Verification verificationWindow = new Verification(selectedReport, _policeObject, _policeUserId);
                verificationWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một biên bản trước.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SendNotificationButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.SelectedItem is Report selectedReport)
            {
                if (selectedReport.Status != "Approved")
                {
                    MessageBox.Show("Bạn chỉ có thể gửi thông báo cho biên bản đã được phê duyệt.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (selectedReport.NotificationSent)
                {
                    MessageBox.Show("Thông báo đã được gửi cho biên bản này.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                PoliceNotification policeNotification = new PoliceNotification(selectedReport, _policeObject, _policeUserId);
                policeNotification.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một biên bản trước.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = FilterDatePicker.SelectedDate;
            string selectedStatus = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string searchText = SearchTextBox.Text.ToLower().Trim();

            var filteredReports = AllReports.Where(report =>
                (selectedDate == null || (report.ReportDate.HasValue && report.ReportDate.Value.Date == selectedDate.Value.Date)) &&
                (selectedStatus == "All" || report.Status == selectedStatus) &&
                (string.IsNullOrEmpty(searchText) ||
                 (report.PlateNumber != null && report.PlateNumber.ToLower().Contains(searchText)))
            ).ToList();

            ReportsDataGrid.ItemsSource = filteredReports;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter_Click(sender, e);
        }
        private void ReportsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportsDataGrid.SelectedItem is Report selectedReport)
            {
                SendNotificationButton.IsEnabled = selectedReport.Status == "Approved";

                VerifyButton.IsEnabled = !selectedReport.NotificationSent;
            }
            else
            {
                SendNotificationButton.IsEnabled = false;
                VerifyButton.IsEnabled = false;
            }
        }

        private void ViewDetail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int reportId)
            {
                var selectedReport = AllReports.FirstOrDefault(r => r.ReportId == reportId);
                if (selectedReport != null)
                {
                    ReportDetailWindow reportDetailWindow = new ReportDetailWindow(selectedReport);
                    reportDetailWindow.Show();
                }
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

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow reportWindow = new ReportWindow(_policeObject, _policeUserId);
            reportWindow.Show();
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
