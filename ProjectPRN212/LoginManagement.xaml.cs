using BusinessObjects;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProjectPRN212
{
    public partial class LoginManagement : Window
    {
        private readonly UserObject _userObject;
        private List<KeyValuePair<DateTime, Tuple<int, string>>> _allLogs;

        private int CurrentPage = 1;
        private const int PageSize = 10;
        private int TotalPages = 1;

        public LoginManagement()
        {
            InitializeComponent();
            _userObject = new UserObject();
            LoadSecurityLogs();
        }

        private void LoadSecurityLogs()
        {
            _allLogs = _userObject.GetLoginLogs();

            TotalPages = (int)Math.Ceiling((double)_allLogs.Count / PageSize);
            DisplayCurrentPage();
        }

        private void DisplayCurrentPage()
        {
            var currentLogs = _allLogs
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(log => new
                {
                    Timestamp = log.Key.ToString("g"),
                    UserId = log.Value.Item1,
                    Action = log.Value.Item2
                }).ToList();

            SecurityLogsGrid.ItemsSource = currentLogs;
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            btnFirst.IsEnabled = CurrentPage > 1;
            btnPrevious.IsEnabled = CurrentPage > 1;
            btnNext.IsEnabled = CurrentPage < TotalPages;
            btnLast.IsEnabled = CurrentPage < TotalPages;
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 1;
            DisplayCurrentPage();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                DisplayCurrentPage();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                DisplayCurrentPage();
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = TotalPages;
            DisplayCurrentPage();
        }

        private void btnBackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow
            {
                Left = this.Left,
                Top = this.Top,
                Width = this.Width,
                Height = this.Height
            };
            adminWindow.Show();
            this.Hide();
        }
    }
}
