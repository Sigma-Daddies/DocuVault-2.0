using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DocuVault
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void Btn_SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Example authentication (you should replace this with actual authentication logic)
            string email = TextBox_Email.Text;
            string password = TextBox_Password.Text; // Assuming this is a PasswordBox

            //if (IsAuthenticated(email, password))
            //{
            // On successful login, navigate to the DashboardPage
                this.NavigationService.Navigate(new DashboardPage());
            //}
            //else
            //{
            //    // Show an error message if authentication fails
            //    MessageBox.Show("Invalid login credentials");
            //}
        }

        private bool IsAuthenticated(string email, string password)
        {
            // Replace with actual authentication logic
            return email == "user@example.com" && password == "password123";
        }

        private void TextBox_Email_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_Password_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private bool isPasswordVisible = false;
        private string _password; // Store the password here
        private bool _isSyncingPassword = false; // Flag to prevent recursive updates


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
            // Handle the click event
            this.NavigationService.Navigate(new ForgotPasswordPage());
        }

        private void Label_Click(object sender, MouseButtonEventArgs e)
        {
            // Handle the click event
            this.NavigationService.Navigate(new RegisterPage());
        }
    }
}
