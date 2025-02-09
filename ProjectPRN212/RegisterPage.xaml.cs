using BusinessObjects;
using DataAccess.Models;
using System;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectPRN212
{
    public partial class RegisterPage : Window
    {
        private readonly UserObject _userObject;
        public RegisterPage()
        {
            InitializeComponent();
            _userObject = new UserObject();
            cboRole.SelectedIndex = 0;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fullName = txtFullName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string role = (cboRole.SelectedItem as ComboBoxItem)?.Content.ToString();
                string address = txtAddress.Text.Trim();
                string password = pwbPass.Password.Trim();
                string confirmPassword = pwbConfirmPass.Password.Trim();

                if (!ValidateInputs(fullName, email, phone, role, address, password, confirmPassword))
                {
                    return;
                }

                if (_userObject.GetUserByEmail(email) != null)
                {
                    MessageBox.Show("Email has already been registered.", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                User newUser = new User
                {
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Role = role,
                    Address = address,
                    Password = password
                };

                string otp = GenerateOTP();
                if (!SendOTPEmail(email, otp))
                {
                    MessageBox.Show("Failed to send verification email. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var otpWindow = new OTPVerification(otp, email, newUser, _userObject);
                otpWindow.ShowDialog();

                if (otpWindow.IsVerified)
                {
                    ClearFields();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            cboRole.SelectedIndex = 0;
            txtAddress.Clear();
            pwbPass.Clear();
            pwbConfirmPass.Clear();
        }

        private bool ValidateInputs(string fullName, string email, string phone, string role, string address, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Full Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                MessageBox.Show("Invalid Email format.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^\d{10,11}$"))
            {
                MessageBox.Show("Phone number must be 10 or 11 digits.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Role is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cboRole.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Address is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAddress.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                pwbPass.Focus();
                return false;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                pwbConfirmPass.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Register_Click(sender, e);
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private bool SendOTPEmail(string toEmail, string otp)
        {
            try
            {
                string fromEmail = "sonpthe170741@fpt.edu.vn";
                string fromPassword = "fspq hcjd nlot mnfw";

                MailMessage message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Verified OTP",
                    Body = $"Your OTP to verify is: <b>{otp}</b><br>Please verify it in app.",
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending OTP: {ex.Message}", "Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}