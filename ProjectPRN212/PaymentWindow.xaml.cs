using DataAccess.Models;
using DataAccess.Repository;
using System.Windows;
using System.Windows.Media;

namespace ProjectPRN212
{
    public partial class PaymentWindow : Window
    {
        private readonly int _userId;
        private readonly ProfileWindow _profileWindow;
        private readonly ViolationRepository _violationRepository;
        private readonly UserRepository _userRepository;
        private decimal _balance;

        public PaymentWindow(int userId, ProfileWindow profileWindow)
        {
            InitializeComponent();
            _userId = userId;
            _profileWindow = profileWindow;
            _violationRepository = new ViolationRepository(new ProjectPrn212Context());
            _userRepository = new UserRepository();
            LoadViolations();
            LoadBalance();
        }

        private void LoadBalance()
        {
            var user = _userRepository.GetUserById(_userId);
            _balance = user.Balance ?? 0;
            BalanceTextBlock.Text = $"Số dư: {_balance:N0} VND";
        }

        private void LoadViolations()
        {
            var violations = _violationRepository.GetViolationsByUserId(_userId);
            ViolationsDataGrid.ItemsSource = violations;
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViolationsDataGrid.SelectedItem is Violation selectedViolation)
            {
                if (selectedViolation.PaidStatus)
                {
                    MessageTextBlock.Text = "Khoản phạt này đã được thanh toán.";
                    MessageTextBlock.Foreground = Brushes.Red;
                    return;
                }

                var user = _userRepository.GetUserById(_userId);
                if (user.Balance < selectedViolation.FineAmount)
                {
                    MessageTextBlock.Text = "Số dư không đủ để thanh toán.";
                    MessageTextBlock.Foreground = Brushes.Red;
                    return;
                }

                selectedViolation.PaidStatus = true;
                user.Balance -= selectedViolation.FineAmount ?? 0;

                var payment = new Payment
                {
                    UserId = _userId,
                    ViolationId = selectedViolation.ViolationId,
                    PaymentMethod = "Cash",
                    Amount = selectedViolation.FineAmount ?? 0,
                    PaymentDate = DateTime.Now
                };

                using (var context = new ProjectPrn212Context())
                {
                    context.Violations.Update(selectedViolation);
                    context.Users.Update(user);
                    context.Payments.Add(payment);
                    context.SaveChanges();
                }

                _profileWindow.UpdateBalance(user.Balance);

                MessageTextBlock.Text = "Thanh toán thành công!";
                LoadViolations();
                LoadBalance();
            }
            else
            {
                MessageTextBlock.Text = "Vui lòng chọn một vi phạm để thanh toán.";
                MessageTextBlock.Foreground = Brushes.Red;
            }
        }
    }
}
