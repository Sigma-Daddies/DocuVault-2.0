using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DocuVault
{
    public partial class LoginPage : Page
    {
        private readonly UserService _userService;
        private bool isPasswordVisible = false;
        private string _password; // Store the password here
        private bool _isSyncingPassword = false; // Flag to prevent recursive updates

        public LoginPage()
        {
            InitializeComponent();
            var accessDB = new AccessDB(); // Initialize AccessDB
            _userService = new UserService(accessDB); // Initialize UserService
        }

        private void Btn_SignIn_Click(object sender, RoutedEventArgs e)
        {
            string email = TextBox_Email.Text;
            string password = _password;

            // Attempt to log in the user
            if (_userService.Login(email, password))
            {
                // Navigate to DashboardPage and pass logged-in user details
                int userId = _userService.GetLoggedInUserId();
                string userEmail = _userService.GetLoggedInUserEmail();
                bool isAdmin = _userService.GetIsAdministrator();

                this.NavigationService.Navigate(new DashboardPage(userId, userEmail, isAdmin));
            }
            else
            {
                MessageBox.Show("Invalid login credentials", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
