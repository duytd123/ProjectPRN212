using System.Net.Mail;
using System.Net;
using System.Windows;
using System.Windows.Input;
using BusinessObjects;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.Repository.Interface;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;

namespace ProjectPRN212
{
    public partial class LoginPage : Window
    {
        private readonly UserObject _userObject;

        private readonly ITrustedDeviceRepository _trustedDeviceRepository;
        private string _currentDeviceToken;
        private readonly IPoliceRepository _policeRepository;
        private readonly PoliceObject _policeObject;
        public LoginPage()
        {
            InitializeComponent();
            _userObject = new UserObject();
            _policeRepository = new PoliceRepository(new ProjectPrn212Context());
            _policeObject = new PoliceObject(_policeRepository);

            _trustedDeviceRepository = new TrustedDeviceRepository(new ProjectPrn212Context());
            _currentDeviceToken = string.Empty;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtEmail.Text;
                string password = pwbPass.Password;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ email và mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                User user = _userObject.GetUserLogin(email, password);
                if (user == null)
                {
                    MessageBox.Show("Email hoặc mật khẩu không chính xác! Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.IsDisabled ?? false)
                {
                    MessageBox.Show("Tài khoản của bạn đã bị vô hiệu hóa! Vui lòng liên hệ cảnh sát để kích hoạt lại.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;

                }

                var configJson = File.ReadAllText("config.json");
                var config = JsonConvert.DeserializeObject<SystemConfig>(configJson);
                bool isLoggingEnabled = config?.EnableLogging ?? false;

                if (isLoggingEnabled && user.Role != "Admin")
                {
                    MessageBox.Show("Hệ thống đang được bảo trì, vui lòng thử lại sau!", "Bảo trì", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (user.Role.Equals("Admin") || user.Role.Equals("TrafficPolice"))
                {
                    OpenUserWindow(user);
                    return;
                }

                if (!IsValidEmail(email))
                {
                    MessageBox.Show("Định dạng email không hợp lệ! Vui lòng nhập đúng định dạng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user != null)
                {
                    _currentDeviceToken = GenerateDeviceToken(user.UserId);
                }


                bool isTwoFactorEnabled = config?.EnableTwoFactorAuth ?? false;
                bool isTrustedDevice = _trustedDeviceRepository.IsDeviceTrusted(user.UserId, _currentDeviceToken);

                if (isTwoFactorEnabled && !isTrustedDevice)
                {
                    string otp = GenerateOtp();
                    SendOtpToEmail(email, otp);

                    int maxAttempts = 3;
                    int attempts = 0;
                    bool isOtpVerified = false;

                    while (attempts < maxAttempts)
                    {
                        isOtpVerified = VerifyOtp(otp);
                        if (isOtpVerified)
                        {
                            break;
                        }

                        attempts++;
                        MessageBox.Show($"Mã OTP không hợp lệ. Còn {maxAttempts - attempts} lần thử.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    if (!isOtpVerified)
                    {
                        MessageBox.Show("Bạn đã nhập sai mã OTP quá số lần cho phép!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (chkTrustDevice.IsChecked == true)
                    {
                        _trustedDeviceRepository.AddTrustedDevice(user.UserId, _currentDeviceToken);
                    }
                }

                OpenUserWindow(user);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenUserWindow(User user)
        {
            if (user.Role == "TrafficPolice")
            {
                int? policeUserId = _policeRepository.GetPoliceUserId(user.Email);
                if (policeUserId == null)
                {
                    MessageBox.Show("Không thể xác định ID của cảnh sát.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                PoliceWindow policeWindow = new PoliceWindow(_policeObject, policeUserId.Value);
                policeWindow.Show();
                this.Close();
                return;
            }
            var roleWindows = new Dictionary<string, Func<Window>>
    {
        { "Citizen", () => new UserWindow(user.UserId) },
        { "Admin", () => new AdminWindow() }
    };

            if (roleWindows.TryGetValue(user.Role, out var windowFactory))
            {
                Window windowToOpen = windowFactory();
                windowToOpen.Show();
                this.Close();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Button_Click(sender, e);
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            ForgotPass forgotPass = new ForgotPass();
            forgotPass.ShowDialog();
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterPage registerPage = new RegisterPage();
            registerPage.ShowDialog();
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
        private string GenerateDeviceToken(int userId)
        {
            string deviceName = Environment.MachineName;
            string userName = Environment.UserName;
            return $"{deviceName}-{userName}-{userId}";

        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        private void SendOtpToEmail(string email, string otp)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("sonpthe170741@fpt.edu.vn");
                mail.To.Add(email);
                mail.Subject = "Mã OTP của bạn";
                mail.Body = $"Mã OTP của bạn là: {otp}";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("sonpthe170741@fpt.edu.vn", "fspq hcjd nlot mnfw");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gửi email OTP thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool VerifyOtp(string otp)
        {
            string inputOtp = Microsoft.VisualBasic.Interaction.InputBox("Nhập mã OTP được gửi đến email của bạn:", "Xác thực OTP", "");
            return inputOtp == otp;
        }

    }
}
