using BusinessObjects;
using DataAccess.Models;
using DataAccess.Repository.Interface;
using DataAccess.Repository;
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
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private readonly ViolationObject _violationObject;
        public ReportWindow()
        {
            InitializeComponent();

            IViolationRepository violationRepository = new ViolationRepository(new ProjectPrn212Context());
            _violationObject = new ViolationObject(violationRepository);

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
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
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
