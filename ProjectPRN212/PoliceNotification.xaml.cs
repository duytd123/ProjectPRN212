using BusinessObjects;
using DataAccess.Models;
using System.Windows;
using System.Windows.Controls;

namespace ProjectPRN212
{
    public partial class PoliceNotification : Window
    {
        private Report _selectedReport;
        private PoliceObject _policeObject;
        private NotifyObject _notifyObject;
        private int _policeUserId;

        public PoliceNotification(Report report, PoliceObject policeObject, int policeUserId)
        {
            InitializeComponent();
            _selectedReport = report;
            _policeObject = policeObject;
            _notifyObject = new NotifyObject();
            _policeUserId = policeUserId;
            LoadReportDetails();
        }

        private void LoadReportDetails()
        {
            PlateNumberTextBlock.Text = _selectedReport.PlateNumber;
            StatusTextBlock.Text = _selectedReport.Status;

            var violator = _policeObject.GetUserByPlateNumber(_selectedReport.PlateNumber);
            ViolatorNameTextBlock.Text = violator?.FullName ?? "Unknown";

            var violation = _policeObject.GetViolationByReportId(_selectedReport.ReportId);
            //if (violation != null)
            //{
            //    FineAmountTextBox.Text = violation.FineAmount.ToString();
            //}
        }

        private void NotificationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isFineNotification = NotificationTypeComboBox.SelectedIndex == 1;
            FineAmountTextBox.IsEnabled = false;
            DueDatePicker.IsEnabled = isFineNotification;

            if (isFineNotification)
            {
                var violation = _policeObject.GetViolationByReportId(_selectedReport.ReportId);
                if (violation != null && violation.ViolationType != null)
                {
                    FineAmountTextBox.Text = violation.ViolationType.FineAmount.ToString();
                }

            }
            else
            {
                FineAmountTextBox.Text = string.Empty;
                DueDatePicker.SelectedDate = null;
            }
        }

        private void SendNotificationButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NotificationMessageTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập nội dung thông báo.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string message = NotificationMessageTextBox.Text;
            var user = _policeObject.GetUserByPlateNumber(_selectedReport.PlateNumber);
            if (user == null)
            {
                MessageBox.Show("Không tìm thấy thông tin người vi phạm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            decimal? fineAmount = null;
            DateTime? dueDate = null;

            if (NotificationTypeComboBox.SelectedIndex == 1)
            {
                var violation = _policeObject.GetViolationByReportId(_selectedReport.ReportId);
                if (violation == null || violation.ViolationType == null)
                {
                    MessageBox.Show("Không thể xác định số tiền phạt.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                fineAmount = violation.ViolationType.FineAmount;

                if (DueDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày hạn nộp phạt.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (DueDatePicker.SelectedDate.Value <= DateTime.Today)
                {
                    MessageBox.Show("Ngày hạn nộp phạt phải sau ngày hôm nay.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                dueDate = DueDatePicker.SelectedDate;
                violation.FineDate = dueDate;
            }

            _policeObject.VerifyAndProcessReport(_selectedReport.ReportId, "Approved", _policeUserId);

            string reporterMessage = $"Đơn phản ánh của bạn về xe biển số {_selectedReport.PlateNumber} đã được duyệt.";
            _notifyObject.AddNotification(_selectedReport.ReporterId, reporterMessage, _selectedReport.PlateNumber);

            string violatorMessage = $"Biển số xe {_selectedReport.PlateNumber} của bạn đã bị phản ánh vi phạm.";
            _notifyObject.AddNotification(user.UserId, violatorMessage, _selectedReport.PlateNumber);

            _policeObject.UpdateReportNotificationStatus(_selectedReport.ReportId, true);
            MessageBox.Show("Xử lý báo cáo và gửi thông báo thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            PoliceWindow policeWindow = new PoliceWindow(_policeObject, _policeUserId);
            policeWindow.Show();
            this.Close();
        }



        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            PoliceWindow policeWindow = new PoliceWindow(_policeObject, _policeUserId);
            policeWindow.Show();
            this.Close();
        }


    }
}
