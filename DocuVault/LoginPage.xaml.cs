using System;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DocuVault
{
    public partial class LoginPage : Page
    {
        // Connection string for MS Access database
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\punza\Desktop\c\DocuVault.accdb";

        private bool isPasswordVisible = false;
        private string _password; // Store the password here
        private bool _isSyncingPassword = false; // Flag to prevent recursive updates

        public LoginPage()
        {
            InitializeComponent();
        }

        private void Btn_SignIn_Click(object sender, RoutedEventArgs e)
        {
            string email = TextBox_Email.Text;
            string password = _password; // Use the stored password from the PasswordBox

            // Authenticate the user
            if (IsAuthenticated(email, password))
            {
                // Retrieve user details (e.g., from the database)
                AppUser user = GetUserFromDatabase(email);

                // On successful login, navigate to the DashboardPage, passing the user object
                this.NavigationService.Navigate(new DashboardPage(user));
            }
            else
            {
                // Show an error message if authentication fails
                MessageBox.Show("Invalid login credentials");
            }
        }

        private bool IsAuthenticated(string email, string password)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Hash the entered password
                    string hashedPassword = HashPassword(password);

                    string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", hashedPassword); // Use the hashed password for comparison

                        int result = (int)command.ExecuteScalar();
                        return result > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return false;
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes); // Return the hashed password as a Base64 string
            }
        }

        private AppUser GetUserFromDatabase(string email)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT Email, IsAdmin FROM Users WHERE Email = @Email";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool isAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"));
                                return new AppUser(email, isAdmin);
                            }
                            else
                            {
                                return null; // User not found
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving user: " + ex.Message);
                    return null;
                }
            }
        }

        private void TextBox_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle email text changes if needed
        }

        private void TextBox_Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle password text changes if needed
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                // Show the password in TextBox and hide PasswordBox
                TextBox_Password.Text = _password; // Sync the stored password to the TextBox
                TextBox_Password.Visibility = Visibility.Visible;
                PasswordBox_Password.Visibility = Visibility.Collapsed;
                ToggleImage.Source = new BitmapImage(new Uri("Icons/eye.png", UriKind.Relative));
            }
            else
            {
                // Show the password in PasswordBox and hide TextBox
                PasswordBox_Password.Password = _password; // Sync the stored password to the PasswordBox
                PasswordBox_Password.Visibility = Visibility.Visible;
                TextBox_Password.Visibility = Visibility.Collapsed;
                ToggleImage.Source = new BitmapImage(new Uri("Icons/eye_closed.png", UriKind.Relative));
            }
        }

        // PasswordBox PasswordChanged handler
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isSyncingPassword)
            {
                _isSyncingPassword = true;
                _password = PasswordBox_Password.Password; // Save the password to the field
                _isSyncingPassword = false;
            }
        }

        // TextBox TextChanged handler
        private void TextBox_Password_TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isSyncingPassword)
            {
                _isSyncingPassword = true;
                _password = TextBox_Password.Text; // Save the password to the field
                _isSyncingPassword = false;
            }
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            // Navigate to ForgotPasswordPage
            this.NavigationService.Navigate(new ForgotPasswordPage());
        }

        private void Label_Click(object sender, MouseButtonEventArgs e)
        {
            // Navigate to RegisterPage
            this.NavigationService.Navigate(new RegisterPage());
        }
    }
}
