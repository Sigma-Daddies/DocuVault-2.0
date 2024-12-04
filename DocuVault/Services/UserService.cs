using System;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DocuVault
{
    public class UserService
    {
        private readonly AccessDB _accessDB;

        // Properties to store the currently logged-in user's details
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public bool IsAdministrator { get; private set; }

        public UserService(AccessDB accessDB)
        {
            _accessDB = accessDB ?? throw new ArgumentNullException(nameof(accessDB));
        }

        // Method to authenticate the user
        public bool Login(string email, string password)
        {
            if (IsAuthenticated(email, password))
            {
                GetLoggedInUserDetails(email); // Fetch and populate user details
                return true;
            }
            return false;
        }

        // Authenticate user credentials
        private bool IsAuthenticated(string email, string password)
        {
            return _accessDB.Execute(connection =>
            {
                string hashedPassword = HashPassword(password);
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    return (int)command.ExecuteScalar() > 0;
                }
            });
        }

        // Fetch the currently logged-in user's details from the database
        private void GetLoggedInUserDetails(string email)
        {
            _accessDB.Execute(connection =>
            {
                string query = "SELECT UserId, Email, IsAdmin FROM Users WHERE Email = @Email";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            Email = reader.GetString(reader.GetOrdinal("Email"));
                            IsAdministrator = reader.GetBoolean(reader.GetOrdinal("IsAdmin"));
                        }
                        else
                        {
                            throw new Exception("User not found.");
                        }
                    }
                }
            });
        }

        // Expose the logged-in user's ID for use in other services
        public int GetLoggedInUserId()
        {
            if (UserId == 0)
                throw new InvalidOperationException("No user is currently logged in.");
            return UserId;
        }

        // Expose the logged-in user's email for use in other services
        public string GetLoggedInUserEmail()
        {
            if (string.IsNullOrEmpty(Email))
                throw new InvalidOperationException("No user is currently logged in.");
            return Email;
        }

        // Expose whether the logged-in user is an administrator
        public bool GetIsAdministrator()
        {
            return IsAdministrator;
        }

        // Method to register a new user
        public async Task<bool> RegisterUserAsync(string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
                throw new ArgumentException("Passwords do not match.");

            string hashedPassword = HashPassword(password);
            return await Task.Run(() =>
            {
                return _accessDB.Execute(connection =>
                {
                    // Check if email already exists
                    string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = ?";
                    using (var checkEmailCommand = new OleDbCommand(checkEmailQuery, connection))
                    {
                        checkEmailCommand.Parameters.Add("?", OleDbType.VarWChar).Value = email;
                        int emailExists = (int)checkEmailCommand.ExecuteScalar();
                        if (emailExists > 0)
                            throw new Exception("This email is already registered.");
                    }

                    // Insert new user into database
                    string insertQuery = "INSERT INTO Users (Email, [Password], IsAdmin) VALUES (?, ?, ?)";
                    using (var insertCommand = new OleDbCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.Add("?", OleDbType.VarWChar).Value = email;
                        insertCommand.Parameters.Add("?", OleDbType.VarWChar).Value = hashedPassword;
                        insertCommand.Parameters.Add("?", OleDbType.Boolean).Value = false;
                        int rowsAffected = insertCommand.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                });
            });
        }

        // Utility method to hash a password
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
