using System.Windows;

namespace DocuVault
{
    public partial class MainWindow : Window
    {
        private UserService _currentUser;

        public MainWindow()
        {
            InitializeComponent();
            NavigateToLoginPage(); // Start by navigating to the login page
        }

        private void NavigateToLoginPage()
        {
            // Navigate to the LoginPage on startup
            LoginPage loginPage = new LoginPage();
            MainFrame.Navigate(loginPage);
        }

        // Event handler for the "Home" button click
        private void NavigateHome(object sender, RoutedEventArgs e)
        {
            NavigateToHomePage(_currentUser);
        }

        // Event handler for the "Manage" button click
        private void NavigateManage(object sender, RoutedEventArgs e)
        {
            NavigateToManagePage();
        }

        // Event handler for the "Audit Trail" button click
        private void NavigateAuditTrail(object sender, RoutedEventArgs e)
        {
            NavigateToAuditTrailPage();
        }

        // Event handler for the "User Management" button click
        private void NavigateUserManagement(object sender, RoutedEventArgs e)
        {
            NavigateToUserManagementPage();
        }

        // Event handler for the "Logout" button click
        private void NavigateLogout(object sender, RoutedEventArgs e)
        {
            NavigateToLogout();
        }

        // These methods are already in place in your current code
        public void NavigateToHomePage(UserService currentUser)
        {
            _currentUser = currentUser; // Store the current user information
            HomePage homePage = new HomePage();
            MainFrame.Navigate(homePage);
        }

        public void NavigateToManagePage()
        {
            if (_currentUser != null)
            {
                ManagePage managePage = new ManagePage(_currentUser);
                MainFrame.Navigate(managePage);
            }
            else
            {
                MessageBox.Show("User session is not valid. Please log in again.");
                NavigateToLoginPage();
            }
        }

        public void NavigateToAuditTrailPage()
        {
            if (_currentUser?.IsAdministrator == true)
            {
                AuditPage auditTrailPage = new AuditPage();
                MainFrame.Navigate(auditTrailPage);
            }
            else
            {
                MessageBox.Show("You do not have permission to access the audit trail.");
            }
        }

        public void NavigateToUserManagementPage()
        {
            if (_currentUser?.IsAdministrator == true)
            {
                UsersPage userManagementPage = new UsersPage();
                MainFrame.Navigate(userManagementPage);
            }
            else
            {
                MessageBox.Show("You do not have permission to access user management.");
            }
        }

        public void NavigateToLogout()
        {
            _currentUser = null;  // Clear user session
            NavigateToLoginPage();  // Redirect to login page
        }
    }
}
