using System.Windows;
using System.Windows.Controls;

namespace DocuVault
{
    public partial class ForgotPasswordPage : Page
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        // Event handler for when the email text changes
        private void TextBox_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Email validation logic or feedback
        }

        // Send verification code
        private void Button_EmailAction_Click(object sender, RoutedEventArgs e)
        {
            // Code to send verification email
            MessageBox.Show("Verification code sent to your email.");
        }

        // Event handler for when the code text changes
        private void TextBox_Code_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Validate code format or feedback
        }

        // Event handler for when the password text changes
        private void PasswordBox_Password_Changed(object sender, RoutedEventArgs e)
        {
            // Password validation logic or feedback
        }

        // Event handler for when the confirm password text changes
        private void PasswordBox_Password_Confirm_Changed(object sender, RoutedEventArgs e)
        {
            // Confirm password matching logic
        }

        // Reset password handler
        private void Btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            // Reset password logic
            MessageBox.Show("Password has been reset successfully.");
        }

        // Back to login page link click handler
        private void BackToLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Navigate to LoginPage
            NavigationService.Navigate(new LoginPage());
        }
    }
}
