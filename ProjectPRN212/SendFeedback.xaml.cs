using BusinessObjects;
using DataAccess.Models;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ProjectPRN212
{
    public partial class SendFeedback : Window
    {
        private string _imagePath;
        private string _videoPath;
        private readonly int _currentUserId;
        private readonly ReportObjects _reportObjects;
        private readonly ViolationObject _violationObject;

        public SendFeedback(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
            _reportObjects = new ReportObjects();
            _violationObject = new ViolationObject();

            btnUploadImage.Click += BtnUploadImage_Click;
            btnUploadVideo.Click += BtnUploadVideo_Click;
            btnSubmit.Click += BtnSubmit_Click;
            LoadViolationTypes();
        }
        private void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg",
                Title = "Select an image file"
            };

            if (openFileDialog.ShowDialog() == true)
            {   
                _imagePath = openFileDialog.FileName;
                txtImageName.Text = Path.GetFileName(_imagePath);
            }
        }
        private void BtnUploadVideo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Video files (*.mp4;*.avi;*.mov)|*.mp4;*.avi;*.mov",
                Title = "Select a video file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _videoPath = openFileDialog.FileName;
                txtVideoName.Text = Path.GetFileName(_videoPath);
            }
        }
        private async void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDescription.Text) ||
                    string.IsNullOrWhiteSpace(txtPlateNumber.Text) ||
                    string.IsNullOrWhiteSpace(txtLocation.Text) ||
                    cbViolationType.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Upload files
                string imageUrl = _reportObjects.UploadFile(_imagePath);
                string videoUrl = _reportObjects.UploadFile(_videoPath);

                // Create report object
                var report = new Report
                {
                    ReporterId = _currentUserId,
                    //ViolationType = ((ComboBoxItem)cbViolationType.SelectedItem).Content.ToString(),
                    Description = txtDescription.Text,
                    PlateNumber = txtPlateNumber.Text,
                    ImageUrl = imageUrl,
                    VideoUrl = videoUrl,
                    Location = txtLocation.Text,
                    ReportDate = DateTime.Now,
                    Status = "Pending"
                };
                bool result = await _reportObjects.AddReport(report);
                if (result)
                {
                    MessageBox.Show("Phản ánh của bạn đã được gửi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không thể gửi phản ánh. Vui lòng thử lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
                catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Đã xảy ra lỗi: {errorMessage}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

                // Log lỗi
                Console.WriteLine($"Exception details: {ex}");
            }
        }

        private async void LoadViolationTypes()
        {
            var violations = await _violationObject.GetAllViolationTypes();
            cbViolationType.ItemsSource = violations;
            cbViolationType.DisplayMemberPath = "ViolationName";
            cbViolationType.SelectedValuePath = "ViolationTypeId";
        }

    }
}