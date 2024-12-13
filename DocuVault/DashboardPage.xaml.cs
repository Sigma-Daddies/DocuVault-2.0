using DocuVault.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace DocuVault
{
    public partial class DashboardPage : Page
    {
        private readonly UserService _userService;
        private int _userId;
        private string _email;
        private bool _isAdmin;

        public DashboardPage(int userId, string email, bool isAdmin)
        {
            InitializeComponent();
            _userId = userId;
            _email = email;
            _isAdmin = isAdmin;

            // Initialize AccessDB (No parameters needed in constructor)
            AccessDB accessDB = new AccessDB(); // Create an instance of AccessDB

            // Pass AccessDB to the UserService constructor
            _userService = new UserService(accessDB);

            // Initialize async
            InitializeAsync();

            Dashboard.Content = new HomePage(); // Set default content to HomePage

            // Show or hide buttons and labels based on user role
            if (_isAdmin)
            {
                // Show admin-related controls
                Button_Audit.Visibility = Visibility.Visible;
                Button_Users.Visibility = Visibility.Visible;
                AdminLabelPanel.Visibility = Visibility.Visible; // Show the admin label
            }
            else
            {
                // Hide admin-related controls
                Button_Audit.Visibility = Visibility.Collapsed;
                Button_Users.Visibility = Visibility.Collapsed;
                AdminLabelPanel.Visibility = Visibility.Collapsed; // Hide the admin label
            }
        }


        // Asynchronous method to initialize data
        private async void InitializeAsync()
        {
            try
            {
                string username = await _userService.GetUsernameAsync(_email);
                UserEmailLabel.Content = $"Welcome, {username}";
            }
            catch (Exception ex)
            {
                // Handle any errors here
                MessageBox.Show($"Error fetching username: {ex.Message}");
            }
        }

        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to ProfilePage
            Dashboard.Navigate(new ProfilePage(_email));
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
