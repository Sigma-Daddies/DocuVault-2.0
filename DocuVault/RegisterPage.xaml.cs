using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace DocuVault
{
    public partial class RegisterPage : Page
    {
        private readonly AccessDB _accessDB;

        public RegisterPage()
        {
            InitializeComponent();
            _accessDB = new AccessDB(); // Initialize AccessDB instance
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

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            string hashedPassword = HashPassword(password);

            try
            {
                await Task.Run(() =>
                {
                    _accessDB.Execute(connection =>
                    {
                        // Check if email already exists
                        string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = ?";
                        using (var checkEmailCommand = new OleDbCommand(checkEmailQuery, connection))
                        {
                            checkEmailCommand.Parameters.Add("?", OleDbType.VarWChar).Value = email;
                            int emailExists = (int)checkEmailCommand.ExecuteScalar();

                            if (emailExists > 0)
                            {
                                throw new Exception("This email is already registered.");
                            }
                        }

                        // Insert new user data into the Users table
                        string insertQuery = "INSERT INTO Users (Email, [Password], IsAdmin) VALUES (?, ?, ?)";
                        using (var insertCommand = new OleDbCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.Add("?", OleDbType.VarWChar).Value = email;
                            insertCommand.Parameters.Add("?", OleDbType.VarWChar).Value = hashedPassword;
                            insertCommand.Parameters.Add("?", OleDbType.Boolean).Value = false;

                            int rowsAffected = insertCommand.ExecuteNonQuery();
                            if (rowsAffected <= 0)
                            {
                                throw new Exception("Registration failed. Please try again.");
                            }
                        }
                    });
                });

                MessageBox.Show("Registration successful!");
                this.NavigationService.Navigate(new LoginPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
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
