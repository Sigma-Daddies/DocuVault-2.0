using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DocuVault.Data;
using DocuVault.Services; // Added namespace for AuditService

namespace DocuVault
{
    public partial class LoginPage : Page
    {
        private readonly UserService _userService;
        private readonly AuditService _auditService; // Added AuditService reference
        private bool isPasswordVisible = false;
        private string _password; // Store the password here
        private bool _isSyncingPassword = false; // Flag to prevent recursive updates

        public LoginPage()
        {
            InitializeComponent();
            var accessDB = new AccessDB(); // Initialize AccessDB
            _userService = new UserService(accessDB); // Initialize UserService
            _auditService = new AuditService(accessDB); // Initialize AuditService
        }

        private async void Btn_SignIn_Click(object sender, RoutedEventArgs e)
        {
            string email = TextBox_Email.Text;
            string password = _password;

            try
            {
                // Use LoginAsync instead of Login
                bool isLoggedIn = await _userService.LoginAsync(email, password); // Update to LoginAsync
                if (isLoggedIn)
                {
                    // Log successful login action
                    await _auditService.LogAuditAsync(_userService.UserID, "User logged in");

                    // Navigate to DashboardPage and pass logged-in user details
                    int userId = _userService.UserID;
                    string userEmail = _userService.Email;
                    bool isAdmin = _userService.IsAdministrator;

                    this.NavigationService.Navigate(new DashboardPage(userId, userEmail, isAdmin));
                }
                else
                {
                    MessageBox.Show("Invalid login credentials", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Show message when account is locked
                MessageBox.Show(ex.Message, "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle email text changes if needed
        }

        private void TextBox_Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle password text changes if needed
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                // Show the password in TextBox and hide PasswordBox
                TextBox_Password.Text = _password; // Sync the stored password to the TextBox
                TextBox_Password.Visibility = Visibility.Visible;
                PasswordBox_Password.Visibility = Visibility.Collapsed;
                ToggleImage.Source = new BitmapImage(new Uri("Icons/eye.png", UriKind.Relative));
            }
            else
            {
                // Show the password in PasswordBox and hide TextBox
                PasswordBox_Password.Password = _password; // Sync the stored password to the PasswordBox
                PasswordBox_Password.Visibility = Visibility.Visible;
                TextBox_Password.Visibility = Visibility.Collapsed;
                ToggleImage.Source = new BitmapImage(new Uri("Icons/eye_closed.png", UriKind.Relative));
            }
        }

        // PasswordBox PasswordChanged handler
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isSyncingPassword)
            {
                _isSyncingPassword = true;
                _password = PasswordBox_Password.Password; // Save the password to the field
                _isSyncingPassword = false;
            }
        }

        // TextBox TextChanged handler
        private void TextBox_Password_TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isSyncingPassword)
            {
                _isSyncingPassword = true;
                _password = TextBox_Password.Text; // Save the password to the field
                _isSyncingPassword = false;
            }
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            // Navigate to ForgotPasswordPage
            this.NavigationService.Navigate(new ForgotPasswordPage());
        }

        private void Label_Click(object sender, MouseButtonEventArgs e)
        {
            // Navigate to RegisterPage
            this.NavigationService.Navigate(new RegisterPage());
        }
    }
}