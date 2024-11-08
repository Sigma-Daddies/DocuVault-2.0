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
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            Dashboard.Content = new HomePage();
        }
        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            Dashboard.Navigate(new ProfilePage());
        }

        private void Button_Home_Click(object sender, RoutedEventArgs e)
        {
            Dashboard.Navigate(new HomePage());
        }

        private void Button_Manage_Click(object sender, RoutedEventArgs e)
        {
            Dashboard.Navigate(new ManagePage());
        }

        private void Button_Audit_Click(object sender, RoutedEventArgs e)
        {
            Dashboard.Navigate(new AuditPage());
        }

        private void Button_Users_Click(object sender, RoutedEventArgs e)
        {
            Dashboard.Navigate(new UsersPage());
        }

        private void Button_Logout_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new LoginPage());
        }




    }
}
