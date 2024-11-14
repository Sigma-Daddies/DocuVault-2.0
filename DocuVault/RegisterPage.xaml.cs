using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;

namespace DocuVault
{
    public partial class RegisterPage : Page
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\punza\Desktop\c\DocuVault.accdb";

        public RegisterPage()
        {
            InitializeComponent();
        }

        // Event handler for TextBox_Email TextChanged
        private void TextBox_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            string email = TextBox_Email.Text;
        }

        // This method handles the click event of the Register button
        private async void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            // Get email and password values from the textboxes
            string email = TextBox_Email.Text;
            string password = PasswordBox_Password.Password;
            string confirmPassword = PasswordBox_Password_Confirm.Password;

            // Basic validation
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

            string hashedPassword = HashPassword(password); // Hash the password before storing

            try
            {
                // Create a connection to the database
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    await conn.OpenAsync(); // Open connection asynchronously
                    // Uncomment for debugging but remove for production to avoid delays
                    // MessageBox.Show("Database connected successfully!");  

                    // Check if email already exists
                    string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = ?";
                    using (OleDbCommand checkEmailCommand = new OleDbCommand(checkEmailQuery, conn))
                    {
                        checkEmailCommand.Parameters.Add("?", OleDbType.VarWChar).Value = email;
                        int emailExists = (int)await checkEmailCommand.ExecuteScalarAsync();

                        if (emailExists > 0)
                        {
                            MessageBox.Show("This email is already registered.");
                            return;
                        }
                    }

                    // Insert new user data into the Users table
                    string insertQuery = "INSERT INTO Users (Email, [Password], IsAdmin) VALUES (?, ?, ?)";
                    using (OleDbCommand insertCommand = new OleDbCommand(insertQuery, conn))
                    {
                        // Add parameters to the command in the correct order
                        insertCommand.Parameters.Add("?", OleDbType.VarWChar).Value = email;    // Email
                        insertCommand.Parameters.Add("?", OleDbType.VarWChar).Value = hashedPassword;  // Hashed Password
                        insertCommand.Parameters.Add("?", OleDbType.Boolean).Value = false;  // IsAdmin (default to false)

                        // Execute the insert command asynchronously
                        int rowsAffected = await insertCommand.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Registration successful!");
                            // Navigate back to the login page after successful registration
                            this.NavigationService.Navigate(new LoginPage());
                        }
                        else
                        {
                            MessageBox.Show("Registration failed. Please try again.");
                        }
                    }
                }
            }
            catch (OleDbException dbEx)
            {
                // More detailed error handling
                MessageBox.Show("Database error: " + dbEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Hash the password using SHA-256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

        // This is the correct event handler for the PasswordChanged event
        private void PasswordBox_Password_Changed(object sender, RoutedEventArgs e)
        {
            // No action needed unless you want to process something when the password changes.
        }

        // This is the correct event handler for the PasswordChanged event for the Confirm Password field
        private void PasswordBox_Password_Confirm_Changed(object sender, RoutedEventArgs e)
        {
            // No action needed unless you want to process something when the confirm password changes.
        }

        // Event handler for the back-to-login button click
        private void BackToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new LoginPage());
        }
    }
}
