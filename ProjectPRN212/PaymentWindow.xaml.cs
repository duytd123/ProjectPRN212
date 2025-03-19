using BusinessObjects;
using DataAccess.Models;
using System.Windows;

namespace ProjectPRN212
{
    public partial class PaymentWindow : Window
    {
        private readonly Violation _violation;
        private readonly User _user;
        private readonly PoliceObject _policeObject;

        public PaymentWindow(Violation violation, User user, PoliceObject policeObject)
        {
            InitializeComponent();
            _violation = violation;
            _user = user;
            _policeObject = policeObject;

            FineAmountTextBlock.Text = $"{_violation.ViolationType.FineAmount} VND";
            BalanceTextBlock.Text = $"{_user.Balance} VND";
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Balance >= _violation.ViolationType.FineAmount)
            {
                // Deduct the fine amount from the user's balance
                _user.Balance -= _violation.ViolationType.FineAmount;

                // Update the payment status
                _policeObject.ProcessResponse(_violation.ViolationId, true);

                MessageBox.Show("Thanh toán thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Số dư không đủ để thanh toán!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
