using BusinessObjects;
using DataAccess.Models;
using DataAccess.Repository;
using Newtonsoft.Json;
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
    /// Interaction logic for ConfigSystem.xaml
    /// </summary>
    public partial class ConfigSystem : Window
    {

        private readonly AdminObject _adminObject;
        private const string ConfigFilePath = "config.json";

        public ConfigSystem()
        {
            InitializeComponent();
            _adminObject = new AdminObject(new AdminRepository(new ProjectPrn212Context()));
            LoadSystemConfig();
        }

        private void LoadSystemConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
                var configJson = File.ReadAllText(ConfigFilePath);
                var config = JsonConvert.DeserializeObject<SystemConfig>(configJson);
                if (config != null)
                {
                    txtSessionTimeout.Text = config.SessionTimeout.ToString();                  
                    chkEnableLogging.IsChecked = config.EnableLogging;
                    chkEnableTwoFactorAuth.IsChecked = config.EnableTwoFactorAuth;
                    chkEnableAutoLogout.IsChecked = config.EnableAutoLogout;
                }
            }
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtSessionTimeout.Text, out int sessionTimeout) || sessionTimeout <= 0 )              
            {
                MessageBox.Show("Vui lòng nhập giá trị hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var config = new SystemConfig
            {
                SessionTimeout = sessionTimeout,
                EnableLogging = chkEnableLogging.IsChecked ?? false,
                EnableTwoFactorAuth = chkEnableTwoFactorAuth.IsChecked ?? false,
                EnableAutoLogout = chkEnableAutoLogout.IsChecked ?? false
            };

            var configJson = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, configJson);

            MessageBox.Show("Cấu hình đã được cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
