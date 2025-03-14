using DataAccess.Models;
using System.Windows;

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private readonly User _currentUser;

        public ProfileWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            txtFullName.Text = _currentUser.FullName;
            txtEmail.Text = _currentUser.Email;
            txtPassword.Text = _currentUser.Password;
            txtRole.Text = _currentUser.Role;
            txtPhone.Text = _currentUser.Phone;
            txtAddress.Text = _currentUser.Address ?? "Không có";
            txtVehicleCount.Text = _currentUser.Vehicles.Count.ToString();
        }
    }
}
