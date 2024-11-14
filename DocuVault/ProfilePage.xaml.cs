using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocuVault
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();

            // Populate profile details (dummy data here)
            TextBox_Username.Text = "John Doe";
            TextBox_Email.Text = "johndoe@example.com";
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            // Example logic for saving changes (e.g., updating profile details)
            string updatedUsername = TextBox_Username.Text;
            string updatedEmail = TextBox_Email.Text;

            // Assuming logic to save these details, e.g., to a database or service
            MessageBox.Show($"Profile updated:\nUsername: {updatedUsername}\nEmail: {updatedEmail}");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Example logic for logout
            // Navigate back to LoginPage or perform logout actions
            this.NavigationService.Navigate(new LoginPage());
        }
    }
}
