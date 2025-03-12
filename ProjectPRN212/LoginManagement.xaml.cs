using BusinessObjects;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for LoginManagement.xaml
    /// </summary>
    public partial class LoginManagement : Window
    {
        private readonly UserObject _userObject;    

        public LoginManagement()
        {
            InitializeComponent();
            _userObject = new UserObject();
            LoadSecurityLogs();
        }      
        private void LoadSecurityLogs()
        {
            
            List<KeyValuePair<DateTime, Tuple<int, string>>> loginLogs = _userObject.GetLoginLogs();

            SecurityLogsGrid.ItemsSource = loginLogs.Select(log => new
            {
                Timestamp = log.Key.ToString("g"), 
                UserId = log.Value.Item1,
                Action = log.Value.Item2
            }).ToList();
        }

        private void btnBackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Left = this.Left;
            adminWindow.Top = this.Top;
            adminWindow.Width = this.Width;
            adminWindow.Height = this.Height;
            adminWindow.Show();
            this.Hide();
        }

       
    }
}
