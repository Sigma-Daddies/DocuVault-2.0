using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DocuVault.Models;
using DocuVault.Data;
using System.Threading.Tasks;

namespace DocuVault
{
    public partial class UsersPage : Page
    {
        private readonly UserService _userService;
        private bool _isAdmin;

        // Updated constructor to accept isAdmin
        public UsersPage(bool isAdmin)
        {
            InitializeComponent();
            _userService = new UserService(new AccessDB());
            _isAdmin = isAdmin;  // Store the isAdmin flag
            this.Loaded += UsersPage_Loaded; // Attach Loaded event
        }

        private async void UsersPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsers(); // Await the call to LoadUsers to avoid the warning
        }

        private async Task LoadUsers() // Change from void to Task
        {
            try
            {
                var users = await _userService.GetAllNonAdminUsersAsync();
                foreach (var client in users)
                {
                    client.LockCommand = new RelayCommand(async () =>
                    {
                        await _userService.LockUserAccountAsync(client.UserID);
                        await LoadUsers(); // Call LoadUsers to refresh the entire list
                    });

                    client.UnlockCommand = new RelayCommand(async () =>
                    {
                        await _userService.UnlockUserAccountAsync(client.UserID);
                        await LoadUsers(); // Call LoadUsers to refresh the entire list
                    });
                }

                UsersDataGrid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}");
            }
        }

        private void RefreshDataGrid()
        {
            UsersDataGrid.ItemsSource = null;
            UsersDataGrid.ItemsSource = UsersDataGrid.ItemsSource;
        }
    }

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
