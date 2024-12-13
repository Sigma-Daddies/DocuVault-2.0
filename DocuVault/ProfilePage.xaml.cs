using System;
using System.Windows;
using System.Windows.Controls;
using DocuVault.Data;  // Assuming AccessDB is in this namespace
using DocuVault.Services;  // Assuming UserService is in this namespace
using DocuVault.Utils;

namespace DocuVault
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private readonly UserService _userService;
        private readonly string _email;  // User's email
        private readonly ToastNotifier _toastNotifier;

        public ProfilePage(string email)
        {
            InitializeComponent();

            _email = email;

            // Initialize AccessDB (No parameters needed in constructor)
            AccessDB accessDB = new AccessDB();  // Create an instance of AccessDB

            // Pass AccessDB to the UserService constructor
            _userService = new UserService(accessDB);
            _toastNotifier = new ToastNotifier(ToasterPanel);
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
                    await _toastNotifier.ShowToastConfirm("Name updated successfully!");
                }
                else
                {
                    await _toastNotifier.ShowToastWarning("Failed to update name.");
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
                await _toastNotifier.ShowToastWarning("Passwords do not match. Please try again.");
                return;
            }

            
            try
            {
                if (await _userService.ResetPasswordAsync(_email, newPassword))
                {
                    await _toastNotifier.ShowToastConfirm("Password reset successfully.");
                }
                else
                {
                    await _toastNotifier.ShowToastWarning("Failed to reset password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                await _toastNotifier.ShowToastWarning($"Error resetting password: {ex.Message}");
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
