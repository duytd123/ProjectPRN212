using BusinessObjects;
using DataAccess.Models;
using System.Windows;

namespace ProjectPRN212
{
    public partial class ViolationListWindow : Window
    {
        private readonly int _userId;
        private readonly ViolationObject _violationObject;

        public ViolationListWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _violationObject = new ViolationObject();
            LoadViolations();
        }

        private void LoadViolations()
        {
            try
            {
                var violations = _violationObject.GetViolationsByViolatorId(_userId);
                ViolationListView.ItemsSource = violations;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedViolation = ViolationListView.SelectedItem as Violation;

            if (selectedViolation != null)
            {
                var violationDetailsWindow = new ViolationDetailsWindow(selectedViolation.ViolationId);
                violationDetailsWindow.ShowDialog();
                LoadViolations();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một vi phạm để xem chi tiết!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}