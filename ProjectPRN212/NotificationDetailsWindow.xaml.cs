using DataAccess.Models;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ProjectPRN212
{
    public partial class NotificationDetailsWindow : Window
    {
        public NotificationDetailsWindow(Report report)
        {
            InitializeComponent();
            LoadReportDetails(report);
        }

        private void LoadReportDetails(Report report)
        {
            ViolationTypeText.Text = $"Loại vi phạm: {report.ViolationType}";
            DescriptionText.Text = $"Mô tả: {report.Description}";
            PlateNumberText.Text = $"Biển số xe: {report.PlateNumber}";
            LocationText.Text = $"Địa điểm: {report.Location}";
            ReportDateText.Text = $"Ngày báo cáo: {report.ReportDate?.ToString("dd/MM/yyyy HH:mm")}";
            StatusText.Text = $"Trạng thái: {report.Status}";

            if (report.Violations.Any())
            {
                var violation = report.Violations.First();
                //FineAmountText.Text = $"Số tiền phạt: {violation.FineAmount}";
                DueDateText.Text = $"Hạn nộp phạt: {violation.FineDate?.ToString("dd/MM/yyyy")}";
            }

            if (!string.IsNullOrEmpty(report.ImageUrl))
            {
                ViolationImage.Source = new BitmapImage(new Uri(report.ImageUrl, UriKind.RelativeOrAbsolute));
            }

            if (!string.IsNullOrEmpty(report.VideoUrl))
            {
                ViolationVideo.Source = new Uri(report.VideoUrl, UriKind.RelativeOrAbsolute);
                ViolationVideo.Play();
            }
        }
    }
}
