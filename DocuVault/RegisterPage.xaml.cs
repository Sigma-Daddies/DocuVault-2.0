using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocuVault
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void TextBox_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle email text changes if needed
        }



        // This is the correct event handler for the PasswordChanged event
        private void PasswordBox_Password_Changed(object sender, RoutedEventArgs e)
        {
            // You can access the password using:
            string password = PasswordBox_Password.Password;
            // Add your logic here, e.g., validating password strength or confirming passwords
        }

        // This is the correct event handler for the PasswordChanged event for the Confirm Password field
        private void PasswordBox_Password_Confirm_Changed(object sender, RoutedEventArgs e)
        {
            // You can access the confirm password using:
            string confirmPassword = PasswordBox_Password_Confirm.Password;
            // Add your logic here, e.g., comparing password and confirm password
        }



        private void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            // Handle register button click event, e.g., user registration logic
        }

        private void BackToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            // Handle the click event
            this.NavigationService.Navigate(new LoginPage());
        }

    }
}
