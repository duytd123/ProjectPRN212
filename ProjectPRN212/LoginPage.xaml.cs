using System.Windows;
using System.Windows.Input;
using BusinessObjects;
using DataAccess.Models;
using DataAccess.Repository;

namespace ProjectPRN212
{
    public partial class LoginPage : Window
    {
        private readonly UserObject _userObject;
        public LoginPage()
        {
            InitializeComponent();
            _userObject = new UserObject();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtEmail.Text;
                string password = pwbPass.Password;

                //Validate
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please enter both email and password!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Attempt Login
                User user = _userObject.GetUserLogin(email, password);
                if (user != null && user.Role.Equals("Citizen"))
                {
                    UserWindow userWindow = new UserWindow();
                    userWindow.Show();
                    this.Close();
                }
                else if (user != null && user.Role.Equals("TrafficPolice"))
                {
                    PoliceWindow policeWindow = new PoliceWindow();
                    policeWindow.Show();
                    this.Close();
                }
                else if (user != null && user.Role.Equals("Admin"))
                {
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid  email or password !!! PLease try again !!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } 
            catch(Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) Button_Click(sender, e);
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            ForgotPass forgotPass = new ForgotPass();
            forgotPass.ShowDialog();
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterPage registerPage =  new RegisterPage();
            registerPage.ShowDialog();
        }

    }
}
