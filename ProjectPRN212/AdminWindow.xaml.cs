using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BusinessObjects;
using DataAccess.Models;
using System.Collections.Generic;
using DataAccess.Repository;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private DispatcherTimer _sessionTimer;
        
        private readonly AdminObject _adminObject;
        public AdminWindow()
        {
            InitializeComponent();
            _adminObject = new AdminObject(new AdminRepository(new ProjectPrn212Context()));
            if (_adminObject.IsAutoLogoutEnabled())
            {
                int timeoutMinutes = _adminObject.GetSessionTimeout();
                StartSessionTimer(timeoutMinutes);
            }
            LoadUsers();
            LoadRoles();
        }

        private void LoadUsers()
        {
            var users = _adminObject.GetAllUsers().Where(u => u.Role != "Admin").ToList();
            UsersGrid.ItemsSource = users;
        }

        private void LoadRoles()
        {
                var roles = _adminObject.GetAllUsers()
                                   .Select(u => u.Role)
                                   .Distinct()
                                   .Where(role => role != "Admin")
                                   .ToList();
                cmbRole.ItemsSource = roles;
            roles.Insert(0, "All");
            cmbFilterRole.ItemsSource = roles;
            cmbFilterRole.SelectedIndex = 0;
        }       
        private void ResetForm()
        {
            txtFullName.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = -1;
            txtPhone.Clear();
            txtAddress.Clear();
        }

        private void UsersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                txtFullName.Text = selectedUser.FullName;
                txtEmail.Text = selectedUser.Email;
                txtPassword.Password = selectedUser.Password;
                cmbRole.Text = selectedUser.Role;
                txtPhone.Text = selectedUser.Phone;
                txtAddress.Text = selectedUser.Address;
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserDialog addUserDialog = new AddUserDialog(_adminObject);
            if (addUserDialog.ShowDialog() == true)
            {
                _adminObject.AddUser(addUserDialog.NewUser);
                MessageBox.Show("Thêm người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadUsers();
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
     
        private void btnUpdateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UsersGrid.SelectedItem is not User selectedUser)
                {
                    MessageBox.Show("Vui lòng chọn người dùng cần cập nhật!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Password) ||
                    string.IsNullOrWhiteSpace(cmbRole.Text) ||
                    string.IsNullOrWhiteSpace(txtPhone.Text) ||
                    string.IsNullOrWhiteSpace(txtAddress.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!IsValidEmail(txtEmail.Text))
                {
                    MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var emailExists = _adminObject.GetAllUsers().Any(u => u.Email == txtEmail.Text && u.UserId != selectedUser.UserId);
                if (emailExists)
                {
                    MessageBox.Show("Email đã tồn tại! Vui lòng chọn email khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                selectedUser.FullName = txtFullName.Text;
                selectedUser.Email = txtEmail.Text;
                selectedUser.Password = txtPassword.Password;
                selectedUser.Role = cmbRole.Text;
                selectedUser.Phone = txtPhone.Text;
                selectedUser.Address = txtAddress.Text;

                _adminObject.UpdateUser(selectedUser);
                MessageBox.Show("Cập nhật người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadUsers();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật user: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnConfigSystem_Click(object sender, RoutedEventArgs e)
        {
            ConfigSystem configApp = new ConfigSystem();
            configApp.Left = this.Left;
            configApp.Top = this.Top;
            configApp.Width = this.Width;
            configApp.Height = this.Height;
            configApp.Show();
            this.Hide();
        }

        private void btnSecutiryLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginManagement login = new LoginManagement();
            login.Left = this.Left;
            login.Top = this.Top;
            login.Width = this.Width;
            login.Height = this.Height;
            login.Show();
            this.Hide();
        }

        private void btnDisableUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UsersGrid.SelectedItem is not User selectedUser)
                {
                    MessageBox.Show("Vui lòng chọn người dùng cần vô hiệu hóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Bạn có chắc chắn muốn vô hiệu hóa tài khoản này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return;
                }

                _adminObject.DisableUser(selectedUser.UserId);
                MessageBox.Show("Tài khoản đã bị vô hiệu hóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi vô hiệu hóa user: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEnableUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UsersGrid.SelectedItem is not User selectedUser)
                {
                    MessageBox.Show("Vui lòng chọn người dùng cần kích hoạt lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!(selectedUser.IsDisabled ?? false))
                {
                    MessageBox.Show("Tài khoản này chưa bị vô hiệu hóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show("Bạn có chắc chắn muốn kích hoạt lại tài khoản này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }

                _adminObject.EnableUser(selectedUser.UserId);
                MessageBox.Show("Tài khoản đã được kích hoạt lại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kích hoạt lại user: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterUsers(object sender, SelectionChangedEventArgs e)
        {
            var filteredUsers = _adminObject.GetAllUsers().Where(u => u.Role != "Admin");

            if (cmbFilterRole.SelectedItem != null && cmbFilterRole.SelectedItem.ToString() != "All")
            {
                filteredUsers = filteredUsers.Where(u => u.Role == cmbFilterRole.SelectedItem.ToString());
            }
            UsersGrid.ItemsSource = filteredUsers.ToList();
        }

        private void FilterUsersName(object sender, TextChangedEventArgs e)
        {
            

            var filteredUsers = _adminObject.GetAllUsers().Where(u => u.Role != "Admin");

            if (!string.IsNullOrWhiteSpace(txtSearchName.Text))
            {
                filteredUsers = filteredUsers.Where(u => u.FullName.Contains(txtSearchName.Text, StringComparison.OrdinalIgnoreCase));
            }
            UsersGrid.ItemsSource = filteredUsers.ToList();
        }

        private void FilterUsersAddress(object sender, TextChangedEventArgs e)
        {
            var filteredUsers = _adminObject.GetAllUsers().Where(u => u.Role != "Admin");

            if (!string.IsNullOrWhiteSpace(txtFilterAddress.Text))
            {
                filteredUsers = filteredUsers.Where(u => u.Address.Contains(txtFilterAddress.Text, StringComparison.OrdinalIgnoreCase));
            }
            UsersGrid.ItemsSource = filteredUsers.ToList();
        }

        private void Grid_MouseDown_ClearForm(object sender, MouseButtonEventArgs e)
        {
            if (!UsersGrid.IsMouseOver)
            {
                ResetForm();
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            _sessionTimer?.Stop();
            MessageBox.Show("Bạn đã đăng xuất thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            this.Close();
        }


        private void StartSessionTimer(int timeoutMinutes)
        {
            _sessionTimer = new DispatcherTimer();
            _sessionTimer.Interval = TimeSpan.FromMinutes(timeoutMinutes);
            _sessionTimer.Tick += SessionTimeoutHandler;
            _sessionTimer.Start();

            this.MouseMove += ResetSessionTimer;
            this.KeyDown += ResetSessionTimer;
        }

        private void ResetSessionTimer(object sender, EventArgs e)
        {
            _sessionTimer.Stop();
            _sessionTimer.Start();
        }

        private void SessionTimeoutHandler(object sender, EventArgs e)
        {
            _sessionTimer?.Stop();
            MessageBox.Show("You have been automatically logged out due to inactivity.", "Session Timeout", MessageBoxButton.OK, MessageBoxImage.Information);
            if (!IsLoginPageOpen())
            {
                LoginPage loginPage = new LoginPage();
                loginPage.Show();
            }
            this.Close();
        }

        private bool IsLoginPageOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginPage)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
