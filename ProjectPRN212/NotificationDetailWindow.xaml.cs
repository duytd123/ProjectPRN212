using DataAccess.Models;
using System.Windows;

namespace ProjectPRN212
{
    public partial class NotificationDetailWindow : Window
    {
        private Notification _notification;

        public NotificationDetailWindow(Notification notification)
        {
            InitializeComponent();
            _notification = notification;

            MessageTextBlock.Text = _notification.Message;
            SentDateTextBlock.Text = _notification.SentDate?.ToString("dd/MM/yyyy HH:mm") ?? "Không xác định";

            if (!string.IsNullOrEmpty(_notification.PlateNumber))
            {
                VehicleInfoPanel.Visibility = Visibility.Visible;
                PlateNumberTextBlock.Text = _notification.PlateNumber;

                if (_notification.PlateNumberNavigation != null)
                {
                    var vehicle = _notification.PlateNumberNavigation;
                    BrandTextBlock.Text = !string.IsNullOrEmpty(vehicle.Brand)
                        ? vehicle.Brand
                        : "Không có thông tin";

                    ModelTextBlock.Text = !string.IsNullOrEmpty(vehicle.Model)
                        ? vehicle.Model
                        : "Không có thông tin";

                    YearTextBlock.Text = vehicle.ManufactureYear.HasValue
                        ? vehicle.ManufactureYear.Value.ToString()
                        : "Không có thông tin";
                }
                else
                {
                    BrandTextBlock.Text = "Không có thông tin";
                    ModelTextBlock.Text = "Không có thông tin";
                    YearTextBlock.Text = "Không có thông tin";
                }
            }
            else
            {
                VehicleInfoPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
