using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocuVault
{
    public partial class RegisterPage : Page
    {
        private readonly UserService _userService;

        public RegisterPage()
        {
            InitializeComponent();
            _userService = new UserService(new AccessDB());
        }

        private void TextBox_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            string email = TextBox_Email.Text;
        }

        private async void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            string email = TextBox_Email.Text;
            string password = PasswordBox_Password.Password;
            string confirmPassword = PasswordBox_Password_Confirm.Password;

            try
            {
                bool isRegistered = await _userService.RegisterUserAsync(email, password, confirmPassword);

                if (isRegistered)
                {
                    MessageBox.Show("Registration successful!");
                    this.NavigationService.Navigate(new LoginPage());
                }
                else
                {
                    MessageBox.Show("Registration failed. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PasswordBox_Password_Changed(object sender, RoutedEventArgs e) 
        {

        }

        private void PasswordBox_Password_Confirm_Changed(object sender, RoutedEventArgs e)
        {

        }

        private void BackToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new LoginPage());
        }
    }
}
