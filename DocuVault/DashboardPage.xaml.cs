using System;
using System.Windows;
using System.Windows.Controls;

namespace DocuVault
{
    public partial class DashboardPage : Page
    {
        private int _userId;
        private string _email;
        private bool _isAdmin;

        public DashboardPage(int userId, string email, bool isAdmin)
        {
            InitializeComponent();
            _userId = userId;
            _email = email;
            _isAdmin = isAdmin;

            Dashboard.Content = new HomePage();  // Set default content to HomePage

            // Show or hide buttons based on user role
            if (_isAdmin)
            {
                // Show the admin-related buttons (Audit and User Management)
                Button_Audit.Visibility = Visibility.Visible;
                Button_Users.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide the admin-related buttons
                Button_Audit.Visibility = Visibility.Collapsed;
                Button_Users.Visibility = Visibility.Collapsed;
            }
        }

        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to ProfilePage
            Dashboard.Navigate(new ProfilePage());
        }

        private void Button_Home_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to HomePage
            Dashboard.Navigate(new HomePage());
        }

        private void Button_Manage_Click(object sender, RoutedEventArgs e)
        {
            // Pass user details to ManagePage
            Dashboard.Navigate(new ManagePage(_userId, _email, _isAdmin));
        }

        private void Button_Audit_Click(object sender, RoutedEventArgs e)
        {
            // Pass the isAdmin flag to AuditPage
            Dashboard.Navigate(new AuditPage(_isAdmin));
        }

        private void Button_Users_Click(object sender, RoutedEventArgs e)
        {
            // Pass the isAdmin flag to UsersPage
            Dashboard.Navigate(new UsersPage(_isAdmin));
        }

        private void Button_Logout_Click_1(object sender, RoutedEventArgs e)
        {
            // Log out and navigate back to LoginPage
            this.NavigationService.Navigate(new LoginPage());
        }
    }
}
