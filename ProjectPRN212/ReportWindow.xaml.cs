using BusinessObjects;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.Repository.Interface;
using System.Windows;

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private readonly ViolationObject _violationObject;
        private PoliceObject _policeObject;
        public ReportWindow(PoliceObject policeObject)
        {
            InitializeComponent();

            IViolationRepository violationRepository = new ViolationRepository(new ProjectPrn212Context());
            _violationObject = new ViolationObject(violationRepository);
            _policeObject = policeObject;
            LoadReportData();
            LoadReportStatistics();

        }

        private void LoadReportData()
        {
            List<Report> reports = _violationObject.GetViolationReports();
            ReportGrid.ItemsSource = reports;
        }

        private void LoadReportStatistics()
        {
            Dictionary<string, int> statistics = _violationObject.GetReportStatistics();

            PendingCount.Text = $"Chờ duyệt: {statistics.GetValueOrDefault("Pending", 0)}";
            ApprovedCount.Text = $"Đã xử lý: {statistics.GetValueOrDefault("Approved", 0)}";
            RejectedCount.Text = $"Bị từ chối: {statistics.GetValueOrDefault("Rejected", 0)}";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            PoliceWindow policeWindow = new PoliceWindow(_policeObject);
            policeWindow.Show();
            this.Close();
        }

        private void btnShowChart_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, int> statistics = _violationObject.GetReportStatistics();
            ReportChartWindow chartWindow = new ReportChartWindow(statistics);
            chartWindow.ShowDialog();
        }
    }
}
