using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DocuVault.Models;
using DocuVault.Data;

namespace DocuVault
{
    public class UserService
    {
        private readonly AccessDB _accessDB;

        // Properties to store the currently logged-in user's details
        public int UserID { get; private set; }
        public string Email { get; private set; }
        public bool IsAdministrator { get; private set; }

        public UserService(AccessDB accessDB)
        {
            _accessDB = accessDB ?? throw new ArgumentNullException(nameof(accessDB));
        }

        // Method to authenticate the user
        public async Task<bool> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Email and password must not be empty.");

            if (await IsAuthenticatedAsync(email, password))
            {
                await GetLoggedInUserDetailsAsync(email); // Fetch and populate user details
                return true;
            }
            return false;
        }

        // Authenticate user credentials asynchronously
        private async Task<bool> IsAuthenticatedAsync(string email, string password)
        {
            return await _accessDB.ExecuteAsync(async connection =>
            {
                string hashedPassword = HashPassword(password);
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password AND IsLocked = False";

                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", hashedPassword);

                    int userCount = (int)await command.ExecuteScalarAsync();
                    return userCount > 0; // User is authenticated and not locked
                }
            });
        }

        // Fetch the currently logged-in user's details asynchronously from the database
        private async Task GetLoggedInUserDetailsAsync(string email)
        {
            await _accessDB.ExecuteAsync(async connection =>
            {
                string query = "SELECT UserID, Email, IsAdmin FROM Users WHERE Email = @Email";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (OleDbDataReader reader = (OleDbDataReader)await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            UserID = reader.GetInt32(reader.GetOrdinal("UserID"));
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

        // Method to register a new user
        public async Task<bool> RegisterUserAsync(string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Email and password must not be empty.");

            if (password != confirmPassword)
                throw new ArgumentException("Passwords do not match.");

            string hashedPassword = HashPassword(password);

            return await _accessDB.ExecuteAsync(async connection =>
            {
                // Check if email already exists
                string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = ?";
                using (var checkEmailCommand = new OleDbCommand(checkEmailQuery, connection))
                {
                    checkEmailCommand.Parameters.Add("?", OleDbType.VarWChar).Value = email;
                    int emailExists = (int)await checkEmailCommand.ExecuteScalarAsync();
                    if (emailExists > 0)
                        throw new Exception("This email is already registered.");
                }

                // Insert new user into database
                string insertQuery = "INSERT INTO Users (Email, [Password], IsAdmin, IsLocked) VALUES (?, ?, ?, ?)";
                using (var insertCommand = new OleDbCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.Add("?", OleDbType.VarWChar).Value = email;
                    insertCommand.Parameters.Add("?", OleDbType.VarWChar).Value = hashedPassword;
                    insertCommand.Parameters.Add("?", OleDbType.Boolean).Value = false; // Default to non-admin
                    insertCommand.Parameters.Add("?", OleDbType.Boolean).Value = false; // Default to unlocked
                    int rowsAffected = await insertCommand.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
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

        // Fetch all non-admin users from the database
        public async Task<List<Client>> GetAllNonAdminUsersAsync()
        {
            var clients = new List<Client>();
            string query = "SELECT UserID, Email, IsLocked FROM Users WHERE IsAdmin = False";

            return await _accessDB.ExecuteAsync(async connection =>
            {
                using (OleDbCommand command = new OleDbCommand(query, connection))
                using (OleDbDataReader reader = (OleDbDataReader)await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        clients.Add(new Client
                        {
                            UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Status = reader.GetBoolean(reader.GetOrdinal("IsLocked")) ? "Locked" : "Active"
                        });
                    }
                }
                return clients;
            });
        }

        // Method to lock a user account
        public async Task LockUserAccountAsync(int userID)
        {
            string query = "UPDATE Users SET IsLocked = True WHERE UserID = ?";
            await _accessDB.ExecuteNonQueryAsync(query, new OleDbParameter("?", userID));
        }

        // Method to unlock a user account
        public async Task UnlockUserAccountAsync(int userID)
        {
            string query = "UPDATE Users SET IsLocked = False WHERE UserID = ?";
            await _accessDB.ExecuteNonQueryAsync(query, new OleDbParameter("?", userID));
        }


        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            var accessDB = new AccessDB();
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";

            try
            {
                // Execute the query using AccessDB's ExecuteScalarAsync method
                int count = await accessDB.ExecuteScalarAsync<int>(query, new OleDbParameter("@Email", email));
                return count > 0;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception("Failed to check if email is registered: " + ex.Message, ex);
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Email and password must not be empty.");

            string query = "UPDATE Users SET [Password] = @NewPasswordHash WHERE [Email] = @Email";

            try
            {
                // Use the HashPassword method to compute the password hash
                string newPasswordHash = HashPassword(newPassword);

                // Execute the query using AccessDB's ExecuteNonQueryAsync method
                int rowsAffected = await _accessDB.ExecuteNonQueryAsync(query,
                    new OleDbParameter("@NewPasswordHash", newPasswordHash),
                    new OleDbParameter("@Email", email));

                if (rowsAffected == 0)
                    throw new Exception("No rows were updated. The email may not exist.");

                return true;
            }
            catch (Exception ex)
            {
                // Log or re-throw the exception as needed
                throw new Exception($"Error resetting password for email '{email}': {ex.Message}", ex);
            }
        }



        private string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
