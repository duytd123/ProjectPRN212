using BusinessObjects;
using DataAccess.Models;
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
    public partial class AddUserDialog : Window
    {
        private readonly AdminObject _adminObject;

        public User NewUser { get; private set; }

        public AddUserDialog(AdminObject adminObject)
        {
            InitializeComponent();
            _adminObject = adminObject;

            cmbRole.ItemsSource = _adminObject.GetAllUsers()
                                              .Select(u => u.Role)
                                              .Distinct()
                                              .Where(role => role != "Admin")
                                              .ToList();
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

                var existingUser = _adminObject.GetAllUsers().FirstOrDefault(u => u.Email == txtEmail.Text);
                if (existingUser != null)
                {
                    MessageBox.Show("Email đã tồn tại! Vui lòng nhập email khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                NewUser = new User
                {
                    FullName = txtFullName.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Password,
                    Role = cmbRole.Text,
                    Phone = txtPhone.Text,
                    Address = txtAddress.Text
                };
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm user: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
