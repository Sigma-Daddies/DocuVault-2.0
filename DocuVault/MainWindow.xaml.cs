using System.Windows;

namespace DocuVault
{
    public partial class MainWindow : Window
    {
        private UserService _currentUserService;

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
            NavigateToHomePage(_currentUserService);
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

        public void NavigateToHomePage(UserService currentUserService)
        {
            _currentUserService = currentUserService;
            HomePage homePage = new HomePage();
            MainFrame.Navigate(homePage);
        }

        public void NavigateToManagePage()
        {
            if (_currentUserService != null)
            {
                ManagePage managePage = new ManagePage(
                    _currentUserService.UserID,
                    _currentUserService.Email,
                    _currentUserService.IsAdministrator
                );
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
            if (_currentUserService?.IsAdministrator == true)
            {
                // Ensure that we don't navigate to the same page repeatedly
                if (!(MainFrame.Content is AuditPage))
                {
                    AuditPage auditPage = new AuditPage(_currentUserService.IsAdministrator);
                    MainFrame.Navigate(auditPage);
                }
            }
            else
            {
                MessageBox.Show("You do not have permission to access the audit trail.");
            }
        }



        public void NavigateToUserManagementPage()
        {
            if (_currentUserService?.IsAdministrator == true)
            {
                UsersPage userManagementPage = new UsersPage(_currentUserService.IsAdministrator);
                MainFrame.Navigate(userManagementPage);
            }
            else
            {
                MessageBox.Show("You do not have permission to access user management.");
            }
        }

        public void NavigateToLogout()
        {
            _currentUserService = null;  // Clear user session
            NavigateToLoginPage();  // Redirect to login page
        }
    }
}
