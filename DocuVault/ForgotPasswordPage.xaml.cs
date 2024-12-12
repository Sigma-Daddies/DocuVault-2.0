using DocuVault.Data;
using DocuVault.Services;
using System.Windows.Controls;
using System.Windows;
using System;

namespace DocuVault
{
    public partial class ForgotPasswordPage : Page
    {
        private readonly EmailService _emailService;
        private readonly UserService _userService;
        private string _verificationPin; // The verification PIN
        private DateTime _pinGeneratedTime; // The time the PIN was generated
        private readonly TimeSpan _pinValidityDuration = TimeSpan.FromMinutes(5);

        public ForgotPasswordPage()
        {
            InitializeComponent();
            _emailService = new EmailService();

            // Initialize UserService with an AccessDB instance
            var accessDB = new AccessDB();
            _userService = new UserService(accessDB);
        }

        private string GenerateVerificationPin()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        // Send verification code
        private async void Button_EmailAction_Click(object sender, RoutedEventArgs e)
        {
            string recipientEmail = TextBox_Email.Text.Trim();

            if (await _userService.IsEmailRegisteredAsync(recipientEmail))
            {
                _verificationPin = GenerateVerificationPin();
                _pinGeneratedTime = DateTime.Now;

                try
                {
                    string plainTextContent = "Please use the following PIN to reset your password.";
                    string htmlContent = "<p>Please use the following PIN to reset your password:</p>";

                    await _emailService.SendEmailAsync(recipientEmail, "Verification PIN", plainTextContent, htmlContent, _verificationPin);
                    MessageBox.Show("Verification PIN sent to your email!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending email: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Email address is not registered.");
                TextBox_Email.Clear();
            }
        }

        private async void Btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (DateTime.Now - _pinGeneratedTime > _pinValidityDuration)
            {
                MessageBox.Show("The PIN has expired. Please request a new one.");
                return;
            }

            string enteredPin = TextBox_Code.Text.Trim();
            if (enteredPin != _verificationPin)
            {
                MessageBox.Show("Invalid PIN. Please try again.");
                return;
            }

            string newPassword = PasswordBox_Password.Password.Trim();
            string confirmNewPassword = PasswordBox_Password_Confirm.Password.Trim();

            if (newPassword != confirmNewPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.");
                return;
            }

            string userEmail = TextBox_Email.Text.Trim();
            try
            {
                if (await _userService.ResetPasswordAsync(userEmail, newPassword))
                {
                    MessageBox.Show("Password reset successfully.");
                    NavigationService.Navigate(new LoginPage());
                }
                else
                {
                    MessageBox.Show("Failed to reset password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting password: {ex.Message}");
            }
        }

        // Back to login page link click handler
        private void BackToLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void PasswordBox_Password_Changed(object sender, RoutedEventArgs e)
        {
            // Password validation logic or feedback
        }

        private void PasswordBox_Password_Confirm_Changed(object sender, RoutedEventArgs e)
        {
            // Confirm password matching logic
        }

        private void TextBox_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Email validation logic or feedback
        }

        private void TextBox_Code_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Code validation logic or feedback
        }
    }
}
