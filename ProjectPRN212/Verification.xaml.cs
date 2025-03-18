using BusinessObjects;
using DataAccess.Models;
using DataAccess.Repository.Interface;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace ProjectPRN212
{
    public partial class Verification : Window
    {
        private Report _selectedReport;
        private PoliceObject _policeObject;
        private int _policeUserId;
        public Verification(Report report, PoliceObject policeObject, int policeUserId)
        {
            InitializeComponent();
            _selectedReport = report;
            _policeObject = policeObject;
            _policeUserId = policeUserId;

            PlateNumberTextBlock.Text = _selectedReport.PlateNumber;
           // ViolationTypeTextBlock.Text = _selectedReport.ViolationType;
            DescriptionTextBlock.Text = _selectedReport.Description;
            LocationTextBlock.Text = _selectedReport.Location;
            ReportDateTextBlock.Text = _selectedReport.ReportDate?.ToString("yyyy-MM-dd HH:mm") ?? "N/A";

            LoadViolationImage();
            LoadViolationVideo();
        }

        private void LoadViolationImage()
        {
            if (!string.IsNullOrEmpty(_selectedReport.ImageUrl))
            {
                try
                {
                    ViolationImage.Source = new BitmapImage(new Uri(_selectedReport.ImageUrl, UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadViolationVideo()
        {
            if (!string.IsNullOrEmpty(_selectedReport.VideoUrl))
            {
                try
                {
                    string videoPath = _selectedReport.VideoUrl;

                    if (!System.IO.File.Exists(videoPath))
                    {
                        MessageBox.Show($"Không tìm thấy tệp video: {videoPath}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                    else
                    {
                        ViolationVideo.Source = new Uri(videoPath, UriKind.Absolute);
                        ViolationVideo.LoadedBehavior = MediaState.Manual;
                        ViolationVideo.Visibility = Visibility.Visible;
                        VideoControls.Visibility = Visibility.Visible;
                        VideoSlider.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải video: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }


        private void UpdateReportStatus(string status)
        {
            try
            {

                if (!IsPlateNumberValid(_selectedReport.PlateNumber))
                {
                    string rejectionReason = "Không tìm thấy biển số phù hợp.";

                    _selectedReport.Status = "Rejected";
                    _selectedReport.RejectionReason = rejectionReason;

                    _policeObject.VerifyAndProcessReport(_selectedReport.ReportId, "Rejected", _selectedReport.ProcessedBy ?? 0, rejectionReason);

                    StatusTextBlock.Text = "REJECTED";
                    StatusTextBlock.Foreground = Brushes.Red;
                    RejectionReasonTextBlock.Text = $"Lý do: {rejectionReason}";
                    RejectionReasonTextBlock.Visibility = Visibility.Visible;

                    MessageBox.Show($"Báo cáo đã bị từ chối do không tìm thấy biển số phù hợp!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _policeObject.VerifyAndProcessReport(_selectedReport.ReportId, status, _selectedReport.ProcessedBy ?? 0);
                MessageBox.Show($"Báo cáo đã được {status}!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);


                StatusTextBlock.Text = status.ToUpper();
                StatusTextBlock.Foreground = Brushes.Red;

                _selectedReport.Status = status;

                if (status != "Rejected")
                {
                    RejectionReasonTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsPlateNumberValid(string plateNumber)
        {
            return _policeObject.DoesVehicleExist(plateNumber);
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                 "Bạn có chắc chắn muốn phê duyệt báo cáo này không?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                UpdateReportStatus("Approved");
            }
        }


        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            InputDialog inputDialog = new InputDialog();
            if (inputDialog.ShowDialog() == true)
            {
                string reason = inputDialog.ResponseText;
                if (string.IsNullOrWhiteSpace(reason))
                {
                    MessageBox.Show("Bạn phải cung cấp lý do từ chối.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    _selectedReport.Status = "Rejected";
                    _selectedReport.RejectionReason = reason;
                    _policeObject.VerifyAndProcessReport(_selectedReport.ReportId, "Rejected", _selectedReport.ProcessedBy ?? 0, reason);

                    MessageBox.Show("Báo cáo đã bị từ chối!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    StatusTextBlock.Text = "REJECTED";
                    StatusTextBlock.Foreground = Brushes.Red;
                    RejectionReasonTextBlock.Text = $"reson:  {reason}";
                    RejectionReasonTextBlock.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi từ chối báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            PoliceWindow policeWindow = new PoliceWindow(_policeObject,_policeUserId);
            policeWindow.Show();
            this.Close();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ViolationVideo.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            ViolationVideo.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            ViolationVideo.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedReport.Status))
            {
                StatusTextBlock.Text = _selectedReport.Status.ToUpper();
                StatusTextBlock.Foreground = Brushes.Red;
            }

            if (_selectedReport.Status == "Rejected" && !string.IsNullOrEmpty(_selectedReport.RejectionReason))
            {
                RejectionReasonTextBlock.Text = $"Lý do:{_selectedReport.RejectionReason}";
                RejectionReasonTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                RejectionReasonTextBlock.Visibility = Visibility.Collapsed;
            }

            ViolationVideo.Play();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (ViolationVideo.NaturalDuration.HasTimeSpan)
            {
                VideoSlider.Maximum = ViolationVideo.NaturalDuration.TimeSpan.TotalSeconds;
                VideoSlider.Value = ViolationVideo.Position.TotalSeconds;
            }
        }

        private void VideoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ViolationVideo.NaturalDuration.HasTimeSpan)
            {
                ViolationVideo.Position = TimeSpan.FromSeconds(VideoSlider.Value);
            }
        }

    }
}
