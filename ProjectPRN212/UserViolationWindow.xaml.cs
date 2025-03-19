using BusinessObjects;
using DataAccess.Models;
using System.Windows;

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for UserViolationWindow.xaml
    /// </summary>
    public partial class UserViolationWindow : Window
    {
        private readonly ViolationObject _violationObject;
        private readonly int _userId;
        private List<Violation> _violations;

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
                _violations = _violationObject.GetViolationsByUserId(_userId);
                ViolationsDataGrid.ItemsSource = _violations;

                if (_violations.Count == 0)
                {
                    MessageBox.Show("Bạn không có vi phạm nào.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                var selectedViolation = ViolationsDataGrid.SelectedItem as Violation;
                if (selectedViolation != null && selectedViolation.IsResponseRejected)
                {
                    RejectedResponseText.Visibility = Visibility.Visible;
                }
                else
                {
                    RejectedResponseText.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViolationsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedViolation = ViolationsDataGrid.SelectedItem as Violation;
            if (selectedViolation != null)
            {
                // Hiển thị chi tiết vi phạm
                ViolationTypeText.Text = selectedViolation.ViolationType?.ViolationName ?? "N/A";
                PlateNumberText.Text = selectedViolation.PlateNumber;
                FineAmountText.Text = selectedViolation.ViolationType?.FineAmount.ToString("N0") + " VND";
                PaidStatusText.Text = selectedViolation.PaidStatus ? "Đã thanh toán" : "Chưa thanh toán";
            }
        }

        private void SubmitResponseButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedViolation = ViolationsDataGrid.SelectedItem as Violation;
            if (selectedViolation != null)
            {
                try
                {
                    string response = ResponseTextBox.Text;
                    if (string.IsNullOrWhiteSpace(response))
                    {
                        MessageBox.Show("Vui lòng nhập phản hồi của bạn.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _violationObject.SubmitResponse(selectedViolation.ViolationId, response);
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
                MessageBox.Show("Vui lòng chọn một vi phạm để gửi phản hồi.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
