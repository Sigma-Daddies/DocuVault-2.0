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
    /// Interaction logic for ForgotPasswordPage.xaml
    /// </summary>
    public partial class ForgotPasswordPage : Page
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
        }


        private void TextBox_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle email text changes if needed
        }

        private void TextBox_Code_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_EmailAction_Click(object sender, RoutedEventArgs e)
        {
            // Your logic for the button click goes here
            MessageBox.Show("Email button clicked!");
        }


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



        private void Btn_Reset_Click(object sender, RoutedEventArgs e)
        {

        }


        private void BackToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            // Handle the click event
            this.NavigationService.Navigate(new LoginPage());
        }

        private void Tex(object sender, TextChangedEventArgs e)
        {

        }


    }
}
