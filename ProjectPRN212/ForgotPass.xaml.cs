using BusinessObjects;
using DataAccess.Models;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for ForgotPass.xaml
    /// </summary>
    public partial class ForgotPass : Window
    {
        private readonly UserObject _userObject;
        public ForgotPass()
        {
            InitializeComponent();
            _userObject = new UserObject();
        }
        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                User user = _userObject.GetUserByEmail(email);
                if (string.IsNullOrEmpty(email) || user == null)
                {
                    MessageBox.Show("Cannot found your email !!! Please try again !!!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    string newPassword = GenerateRandomPassword();

                    user.Password = newPassword;
                    _userObject.UpdateUser(user);

                    bool emailSent = SendResetPasswordEmail(email, newPassword);
                    if (emailSent)
                    {
                        MessageBox.Show("A new password has been sent to your email!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to send email!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                
            }
            catch(Exception ex)
            {

            }
        }
        private string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString().Substring(0, 8); // 8 ký tự ngẫu nhiên
        }
        private bool SendResetPasswordEmail(string toEmail, string newPassword)
        {
            try
            {
                string fromEmail = "sonpthe170741@fpt.edu.vn";
                string fromPassword = "fspq hcjd nlot mnfw";

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromEmail);
                message.To.Add(toEmail);
                message.Subject = "Password Reset";
                message.Body = $"Your new password is: <b>{newPassword}</b><br>Please change it after login.";
                message.IsBodyHtml = true;

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
                MessageBox.Show($"Error sending email: {ex.Message}", "Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ResetPassword_Click(sender, e);
        }
    }
}
