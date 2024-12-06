using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DocuVault.Models;
using DocuVault.Data;

namespace DocuVault
{
    public partial class UsersPage : Page
    {
        private readonly UserService _userService;

        public UsersPage()
        {
            InitializeComponent();

            // Instantiate AccessDB without the connection string
            _userService = new UserService(new AccessDB());

            LoadUsers();
        }

        private async void LoadUsers()
        {
            try
            {
                // Fetch all users excluding admins
                var users = await _userService.GetAllNonAdminUsersAsync();

                // Assign lock/unlock commands to each client
                foreach (var client in users)
                {
                    client.LockCommand = new RelayCommand(async () =>
                    {
                        await _userService.LockUserAccountAsync(client.UserID);
                        client.Status = "Locked";
                        RefreshDataGrid();
                    });

                    client.UnlockCommand = new RelayCommand(async () =>
                    {
                        await _userService.UnlockUserAccountAsync(client.UserID);
                        client.Status = "Unlocked";
                        RefreshDataGrid();
                    });
                }

                UsersDataGrid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}");
            }
        }

        // Refresh the DataGrid to reflect updated statuses
        private void RefreshDataGrid()
        {
            UsersDataGrid.ItemsSource = null;
            UsersDataGrid.ItemsSource = UsersDataGrid.ItemsSource;
        }
    }

    // RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
