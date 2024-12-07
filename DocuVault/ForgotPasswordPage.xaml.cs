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
            string recipientEmail = TextBox_Email.Text.Trim(); // Get the input and trim whitespace

            // Check if the email is registered
            if (await _userService.IsEmailRegisteredAsync(recipientEmail))
            {
                _verificationPin = GenerateVerificationPin(); // Generate a random PIN
                _pinGeneratedTime = DateTime.Now; // Store the generation time

                try
                {
                    await _emailService.SendEmailAsync(recipientEmail, "VERIFICATION PIN", "OOP DMS PIN", "<strong></strong>", _verificationPin);
                    MessageBox.Show("Email sent with your verification PIN!");
                }
                catch (Exception ex)
                {
                    // Log or handle the error
                    MessageBox.Show($"Error sending email: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Email address not registered.");
                TextBox_Email.Clear();
            }
        }

        // Generate a random 6-digit verification code
        private async void Btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            string enteredPin = TextBox_Code.Text.Trim();
            string newPassword = PasswordBox_Password.Password.Trim();
            string confirmNewPassword = PasswordBox_Password_Confirm.Password.Trim();

            // Ensure passwords match before resetting
            if (newPassword != confirmNewPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.");
                return;
            }

            // Check if the PIN is still valid
            if (DateTime.Now - _pinGeneratedTime <= _pinValidityDuration)
            {
                if (enteredPin == _verificationPin)
                {
                    string userEmail = TextBox_Email.Text.Trim(); // Retrieve user email

                    // Reset the password using the existing service
                    bool isPasswordChanged = await _userService.ResetPasswordAsync(userEmail, newPassword);

                    if (isPasswordChanged)
                    {
                        MessageBox.Show("Password changed successfully!");
                        // Clear fields and navigate back to login or email field for another reset
                        TextBox_Email.Clear();
                        TextBox_Code.Clear();
                        PasswordBox_Password.Clear();
                        PasswordBox_Password_Confirm.Clear();
                        NavigationService.Navigate(new LoginPage());  // Optional: navigate to login page
                    }
                    else
                    {
                        MessageBox.Show("Failed to change the password. Please try again.");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid PIN. Please try again.");
                }
            }
            else
            {
                MessageBox.Show("The PIN has expired. Please request a new one.");
            }
        }

        // Back to login page link click handler
        private void BackToLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Navigate to LoginPage
            NavigationService.Navigate(new LoginPage());
        }

        private void PasswordBox_Password_Changed(object sender, RoutedEventArgs e)
        {
            // Password validation logic or feedback
        }

        // Event handler for when the confirm password text changes
        private void PasswordBox_Password_Confirm_Changed(object sender, RoutedEventArgs e)
        {
            // Confirm password matching logic
        }

        private void TextBox_Email_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_Code_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
