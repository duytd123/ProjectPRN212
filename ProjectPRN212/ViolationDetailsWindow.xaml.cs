using BusinessObjects;
using DataAccess.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ProjectPRN212
{
    public partial class ViolationDetailsWindow : Window
    {
        private readonly Violation? _violation;
        private readonly Report? _report;
        private readonly User? _violator;
        private readonly User? _reporter;
        private readonly ViolationObject _violationObject;

        public ViolationDetailsWindow(int violationId)
        {
            InitializeComponent();
            _violationObject = new ViolationObject();
            _violation = _violationObject.GetViolationById(violationId);
            if (_violation == null)
            {
                MessageBox.Show("Không thể tải thông tin vi phạm!", "Lỗi",MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            _report = new ReportObjects().GetReportById(_violation.ReportId).Result;
            _violator = new UserObject().GetUserById(_violation.ViolatorId.Value);
            _reporter = new UserObject().GetUserById(_report.ReporterId);

            LoadViolationDetails(); 

            UpdatePaymentStatusUI();
        }

        private void LoadViolationDetails()
        {
            ViolationType.Text = _report.ViolationType;
            PlateNumber.Text = _violation.PlateNumber;
            ViolationLocation.Text = _report.Location;
            FineAmount.Text = string.Format("{0:N0} VNĐ", _violation.FineAmount);

            if (!string.IsNullOrEmpty(_report.ImageUrl))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(_report.ImageUrl, UriKind.RelativeOrAbsolute));
                    EvidenceImage.Source = bitmap;
                    NoImageText.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    NoImageText.Visibility = Visibility.Visible;
                }
            }
            else
            {
                NoImageText.Visibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(_report.VideoUrl))
            {
                PlayVideoButton.Visibility = Visibility.Visible;
                NoVideoText.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoVideoText.Visibility = Visibility.Visible;
                PlayVideoButton.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdatePaymentStatusUI()
        {
            if (_violation.PaidStatus ?? false)
            {
                ProcessPaymentButton.IsEnabled = false;
                ProcessPaymentButton.Content = "Đã nộp phạt";
                ProcessPaymentButton.Background = System.Windows.Media.Brushes.Gray;
            }
        }

        private void PlayVideoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _report.VideoUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở video: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal amountToPay;
                if (!decimal.TryParse(FineAmountTextBox.Text, out amountToPay))
                {
                    MessageBox.Show("Vui lòng nhập số tiền hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (amountToPay > _violation.FineAmount)
                {
                    MessageBox.Show("Số tiền nộp phạt không được vượt quá số tiền phạt!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                _violation.PaidStatus = true;
                _violationObject.UpdateViolation(_violation);
                MessageBox.Show("Thanh toán thành công!", "Thông báo",MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

