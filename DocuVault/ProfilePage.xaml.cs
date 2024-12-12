using System;
using System.Windows;
using System.Windows.Controls;
using DocuVault.Data;  // Assuming AccessDB is in this namespace
using DocuVault.Services;  // Assuming UserService is in this namespace

namespace DocuVault
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private readonly UserService _userService;
        private readonly string _email;  // User's email

        public ProfilePage(string email)
        {
            InitializeComponent();

            _email = email;

            // Initialize AccessDB (No parameters needed in constructor)
            AccessDB accessDB = new AccessDB();  // Create an instance of AccessDB

            // Pass AccessDB to the UserService constructor
            _userService = new UserService(accessDB);
        }

        private async void Btn_ApplyName_Click(object sender, RoutedEventArgs e)
        {
            string username = TextBox_Name.Text.Trim();  // Get name from TextBox

            try
            {
                int userId = await _userService.GetLoggedInUserIdAsync(_email);
                bool result = await _userService.UpdateUsernameAsync(userId, username);

                if (result)
                {
                    MessageBox.Show("Name updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Btn_ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = PasswordBox_New_Password.Password.Trim();
            string confirmNewPassword = PasswordBox_Password_Confirm_New.Password.Trim();

            if (newPassword != confirmNewPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.");
                return;
            }

            
            try
            {
                if (await _userService.ResetPasswordAsync(_email, newPassword))
                {
                    MessageBox.Show("Password reset successfully.");
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
    

        private void TextBox_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle text changes in the Name TextBox if needed
        }

        private void PasswordBox_New_Password_Changed(object sender, RoutedEventArgs e)
        {
            // Handle new password changes here
        }

        private void PasswordBox_Password_Confirm_New_Changed(object sender, RoutedEventArgs e)
        {
            // Handle confirm password changes here
        }
    }
}
