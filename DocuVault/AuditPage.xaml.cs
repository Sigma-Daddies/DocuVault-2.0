using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DocuVault.Data;
using DocuVault.Models;
using DocuVault.Services;

namespace DocuVault
{
    public partial class AuditPage : Page
    {
        private readonly AuditService _auditService;
        private readonly UserService _userService;
        private readonly bool _isAdmin;

        public AuditPage(bool isAdmin)
        {
            InitializeComponent();
            _isAdmin = isAdmin;
            _auditService = new AuditService(new AccessDB());
            _userService = new UserService(new AccessDB());

            LoadUsers();
            LoadAuditLogs();
        }

        private async void LoadUsers()
        {
            try
            {
                var users = _isAdmin
                    ? await _userService.GetAllUsersIncludingAdminsAsync()
                    : await _userService.GetAllNonAdminUsersAsync();
                UserComboBox.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading users: " + ex.Message);
            }
        }


        private async void LoadAuditLogs()
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsWithUserDetailsAsync(_isAdmin);
                AuditDataGrid.ItemsSource = auditLogs;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading audit logs: " + ex.Message);
            }
        }


        private async void UserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserComboBox.SelectedItem is Client selectedUser)
            {
                int userId = selectedUser.UserID;
                await LoadAuditLogsForUser(userId);
            }
        }

        private async System.Threading.Tasks.Task LoadAuditLogsForUser(int userId)
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsByUserAsync(userId);
                AuditDataGrid.ItemsSource = auditLogs;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading audit logs: " + ex.Message);
            }
        }
    }
}
