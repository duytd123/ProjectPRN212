using BusinessObjects;
using DataAccess.Models;
using System.Windows;
using System.Windows.Controls;

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for TrackStatusWindow.xaml
    /// </summary>
    public partial class TrackStatusWindow : Window
    {
        private readonly ReportObjects _reportObject;
        private readonly User _currentUser;

        public TrackStatusWindow(User currentUser)
        {
            InitializeComponent();
            _reportObject = new ReportObjects();
            _currentUser = currentUser;

            // Set default status selection
            cbStatus.SelectedIndex = 0;
            cbViolationType.SelectedIndex = 0;

            // Set default date range (last 30 days)
            dpFromDate.SelectedDate = DateTime.Now.AddDays(-30);
            dpToDate.SelectedDate = DateTime.Now;

            // Load data
            LoadReports();
            btnFilter.Click += BtnFilter_Click;
        }

        private void LoadReports()
        {
            try
            {
                DateOnly? fromDate = dpFromDate.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(dpFromDate.SelectedDate.Value)
                    : null;

                DateOnly? toDate = dpToDate.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(dpToDate.SelectedDate.Value)
                    : null;

                string status = ((ComboBoxItem)cbStatus.SelectedItem)?.Content?.ToString();
                string violationType = ((ComboBoxItem)cbViolationType.SelectedItem)?.Content?.ToString();
                string plateNumber = txtPlateNumber.Text?.Trim();

                var reports = _reportObject.GetReportsByUserIdAndFilters(
                    _currentUser.UserId,
                    fromDate,
                    toDate,
                    status == "Tất cả" ? null : status,
                    violationType == "Tất cả" ? null : violationType,
                    plateNumber
                );

                dgReports.ItemsSource = reports;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi tải dữ liệu: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            // Validate date range
            if (dpFromDate.SelectedDate.HasValue && dpToDate.SelectedDate.HasValue)
            {
                if (dpFromDate.SelectedDate.Value > dpToDate.SelectedDate.Value)
                {
                    MessageBox.Show(
                        "Ngày bắt đầu không thể lớn hơn ngày kết thúc!",
                        "Lỗi",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }
            }

            LoadReports();
        }
    }
}
