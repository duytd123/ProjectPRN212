using BusinessObjects;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.Repository.Interface;
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

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : Window
    {
        private Report _selectedReport;
        private PoliceObject _policeObject;
        public Notification(Report report, PoliceObject policeObject)
        {
            InitializeComponent();
            _selectedReport = report;
            _policeObject = policeObject;
            LoadReportDetails();
        }
        private void LoadReportDetails()
        {
            PlateNumberTextBlock.Text = _selectedReport.PlateNumber;
            StatusTextBlock.Text = _selectedReport.Status;

            var violator = _policeObject.GetUserByPlateNumber(_selectedReport.PlateNumber);
            ViolatorNameTextBlock.Text = violator?.FullName ?? "Unknown";
        }

        private void NotificationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotificationTypeComboBox.SelectedIndex == 1) 
            {
                FineAmountTextBox.IsEnabled = true;
                DueDatePicker.IsEnabled = true;
            }
            else
            {
                FineAmountTextBox.IsEnabled = false;
                DueDatePicker.IsEnabled = false;
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
                if (!decimal.TryParse(FineAmountTextBox.Text, out decimal fine) || fine <= 0)
                {
                    MessageBox.Show("Số tiền phạt không hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DueDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày hạn nộp phạt.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                fineAmount = fine;
                dueDate = DueDatePicker.SelectedDate;
            }

            _policeObject.NotifyViolator(user.UserId, message, _selectedReport.PlateNumber, fineAmount, dueDate);

            MessageBox.Show("Gửi thông báo thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            PoliceWindow policeWindow = new PoliceWindow(_policeObject);
            policeWindow.Show();
            this.Close();
        }
    }
}
