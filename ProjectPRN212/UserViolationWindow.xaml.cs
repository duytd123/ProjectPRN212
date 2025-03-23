using BusinessObjects;
using DataAccess.Models;
using System.Windows;
using System.Windows.Controls;

namespace ProjectPRN212
{
    public partial class UserViolationWindow : Window
    {
        private readonly ViolationObject _violationObject;
        private readonly int _userId;

        public UserViolationWindow(int userId)
        {
            InitializeComponent();
            _violationObject = new ViolationObject();
            _userId = userId;
            LoadViolations();
        }

        private void LoadViolations()
        {
            try
            {
                var violations = _violationObject.GetViolationsByUserId(_userId);

                if (violations == null || !violations.Any())
                {
                    MessageBox.Show("Không có vi phạm nào được tìm thấy.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ViolationsDataGrid.ItemsSource = violations;
                SubmitResponseButton.Visibility = violations.Any() ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViolationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViolationsDataGrid.SelectedItem is Violation selectedViolation)
            {
                if (selectedViolation.Report == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin báo cáo liên quan.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ViolationTypeText.Text = selectedViolation.ViolationType?.ViolationName ?? "N/A";
                PlateNumberText.Text = selectedViolation.PlateNumber ?? "N/A";
                FineAmountText.Text = selectedViolation.FineAmount.HasValue ? $"{selectedViolation.FineAmount.Value} VND" : "N/A";
                PaidStatusText.Text = selectedViolation.PaidStatus ? "Đã thanh toán" : "Chưa thanh toán";

                if (selectedViolation.Report.Status == "Rejected" && !string.IsNullOrEmpty(selectedViolation.Report.RejectionReason))
                {
                    RejectedResponseText.Text = $"Phản hồi của bạn đã bị từ chối. Lý do: {selectedViolation.Report.RejectionReason}";
                    RejectedResponseText.Visibility = Visibility.Visible;
                }
                else
                {
                    RejectedResponseText.Visibility = Visibility.Collapsed;
                }

                if (selectedViolation.ResponseCount > 0)
                {
                    ResponseTextBox.IsEnabled = false;
                    ResponseTextBox.Text = selectedViolation.Response ?? "";
                }
                else
                {
                    ResponseTextBox.IsEnabled = true;
                    ResponseTextBox.Text = "";
                }
            }
        }

        private void SubmitResponseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViolationsDataGrid.SelectedItem is Violation selectedViolation)
            {
                if (selectedViolation.ResponseCount > 0)
                {
                    MessageBox.Show("Bạn chỉ có thể phản hồi một lần.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                string response = ResponseTextBox.Text.Trim();

                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                try
                {
                    _violationObject.UpdateViolationResponse(selectedViolation.ViolationId, response);
                    MessageBox.Show("Phản hồi của bạn đã được gửi thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadViolations();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi gửi phản hồi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một vi phạm để phản hồi.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
