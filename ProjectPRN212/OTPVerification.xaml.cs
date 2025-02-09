using BusinessObjects;
using DataAccess.Models;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace ProjectPRN212
{
    public partial class OTPVerification : Window
    {
        private string _correctOTP;
        private readonly string _userEmail;
        private readonly User _newUser;
        private readonly UserObject _userObject;

        public bool IsVerified { get; private set; } = false;

        public OTPVerification(string otp, string email, User user, UserObject userObject)
        {
            InitializeComponent();
            _correctOTP = otp;
            _userEmail = email;
            _newUser = user;
            _userObject = userObject;
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            string enteredOTP = txtOTP.Text.Trim();

            if (string.IsNullOrEmpty(enteredOTP))
            {
                MessageBox.Show("Please enter the OTP.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (enteredOTP == _correctOTP)
            {
                IsVerified = true;
                _userObject.AddUser(_newUser);
                MessageBox.Show("Email verified successfully! You can now login.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid OTP. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtOTP.Clear();
            }
        }

        private void Resend_Click(object sender, RoutedEventArgs e)
        {
            string newOTP = GenerateOTP();
            if (SendOTPEmail(_userEmail, newOTP))
            {
                _correctOTP = newOTP;
                MessageBox.Show("New OTP has been sent to your email.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

