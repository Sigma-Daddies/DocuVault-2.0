using System;
using System.Windows;
using System.Windows.Controls;

namespace DocuVault
{
    public partial class DashboardPage : Page
    {
        private AppUser currentUser;

        public DashboardPage(AppUser user)
        {
            InitializeComponent();
            currentUser = user;
            Dashboard.Content = new HomePage();  // Set default content to HomePage

            // Show or hide buttons based on user role
            if (currentUser.IsAdministrator)
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
            // Pass the currentUser object to ManagePage
            Dashboard.Navigate(new ManagePage(currentUser));
        }

        private void Button_Audit_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to AuditPage (admin-only access)
            Dashboard.Navigate(new AuditPage());
        }

        private void Button_Users_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to UsersPage (admin-only access)
            Dashboard.Navigate(new UsersPage());
        }

        private void Button_Logout_Click_1(object sender, RoutedEventArgs e)
        {
            // Log out and navigate back to LoginPage
            this.NavigationService.Navigate(new LoginPage());
        }
    }
}
