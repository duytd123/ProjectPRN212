using BusinessObjects;
using DataAccess.Models;
using System.Windows;

namespace ProjectPRN212
{
    public partial class ProfileWindow : Window
    {
        private readonly UserObject _userObject;
        private readonly User _currentUser;

        public ProfileWindow(int userId)
        {
            InitializeComponent();
            _userObject = new UserObject();
            _currentUser = _userObject.GetUserById(userId);
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (_currentUser != null)
            {
                txtFullName.Text = _currentUser.FullName;
                txtEmail.Text = _currentUser.Email;
                pwdPassword.Password = _currentUser.Password;
                pwdConfirmPassword.Password = _currentUser.Password;
                txtPhone.Text = _currentUser.Phone;
                txtAddress.Text = _currentUser.Address;
                txtBalance.Text = _currentUser.Balance.HasValue ? $"{_currentUser.Balance.Value:N0} VND" : "0 VND"; 
            }
            else
            {
                MessageBox.Show("Không thể tải thông tin người dùng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null)
            {
                string newPassword = chkShowPassword.IsChecked == true ? txtPassword.Text : pwdPassword.Password;
                string confirmPassword = chkShowPassword.IsChecked == true ? txtConfirmPassword.Text : pwdConfirmPassword.Password;

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _currentUser.FullName = txtFullName.Text;
                _currentUser.Password = newPassword;
                _currentUser.Phone = txtPhone.Text;
                _currentUser.Address = txtAddress.Text;

                _userObject.UpdateUser(_currentUser);
                MessageBox.Show("Thông tin đã được cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtPassword.Text = pwdPassword.Password;
            txtPassword.Visibility = Visibility.Visible;
            pwdPassword.Visibility = Visibility.Collapsed;

            txtConfirmPassword.Text = pwdConfirmPassword.Password;
            txtConfirmPassword.Visibility = Visibility.Visible;
            pwdConfirmPassword.Visibility = Visibility.Collapsed;
        }

        private void chkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            pwdPassword.Password = txtPassword.Text;
            pwdPassword.Visibility = Visibility.Visible;
            txtPassword.Visibility = Visibility.Collapsed;

            pwdConfirmPassword.Password = txtConfirmPassword.Text;
            pwdConfirmPassword.Visibility = Visibility.Visible;
            txtConfirmPassword.Visibility = Visibility.Collapsed;
        }

        public void UpdateBalance(decimal? newBalance)
        {
            txtBalance.Text = newBalance.HasValue ? $"{newBalance.Value:N0} VND" : "0 VND";
        }
    }
}
