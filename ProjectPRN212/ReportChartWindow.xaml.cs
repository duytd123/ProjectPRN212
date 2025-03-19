using LiveCharts.Wpf;
using LiveCharts;
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
    /// Interaction logic for ReportChartWindow.xaml
    /// </summary>
    public partial class ReportChartWindow : Window
    {
        public ReportChartWindow(Dictionary<string, int> statistics)
        {
            InitializeComponent();
            LoadChartData(statistics);
            DataContext = this;
        }

        private void LoadChartData(Dictionary<string, int> statistics)
        {
            if (ReportChart == null)
            {
                MessageBox.Show("Report chưa được khởi tạo!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var labels = statistics.Keys.ToList();
            var values = statistics.Values.Select(v => (double)v).ToList();

            ReportChart.AxisX.Clear();
            ReportChart.AxisX.Add(new Axis
            {
                Title = "Trạng thái",
                Labels = labels
            });

            ReportChart.AxisY.Clear();
            ReportChart.AxisY.Add(new Axis
            {
                Title = "Số lượng báo cáo",
                MinValue = 0, 
                Separator = new LiveCharts.Wpf.Separator { Step = 1 } 
            });

            ReportChart.Series.Clear();
            ReportChart.Series.Add(new ColumnSeries
            {
                Title = "Báo cáo",
                Values = new ChartValues<double>(values)
            });
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
