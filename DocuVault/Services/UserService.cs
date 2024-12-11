using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DocuVault.Models;
using DocuVault.Data;
using System.Windows;

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
                await GetLoggedInUserDetailsAsync(email);
                return true;
            }
            return false;
        }

        // Authenticate user credentials
        private async Task<bool> IsAuthenticatedAsync(string email, string password)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password AND IsLocked = False";
            string hashedPassword = HashPassword(password);

            return await _accessDB.ExecuteScalarAsync<int>(query,
                new OleDbParameter("@Email", email),
                new OleDbParameter("@Password", hashedPassword)) > 0;
        }

        // Fetch the currently logged-in user's details
        private async Task GetLoggedInUserDetailsAsync(string email)
        {
            string query = "SELECT UserID, Email, IsAdmin FROM Users WHERE Email = @Email";
            await _accessDB.ExecuteAsync(async connection =>
            {
                using (var command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = await command.ExecuteReaderAsync())
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

        // Register a new user
        public async Task<bool> RegisterUserAsync(string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || password != confirmPassword)
                throw new ArgumentException("Invalid registration details.");

            string hashedPassword = HashPassword(password);
            string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = ?";

            if (await _accessDB.ExecuteScalarAsync<int>(checkEmailQuery, new OleDbParameter("?", email)) > 0)
                throw new Exception("This email is already registered.");

            string insertQuery = "INSERT INTO Users (Email, [Password], IsAdmin, IsLocked) VALUES (?, ?, ?, ?)";
            return await _accessDB.ExecuteNonQueryAsync(insertQuery,
                new OleDbParameter("?", email),
                new OleDbParameter("?", hashedPassword),
                new OleDbParameter("?", false),
                new OleDbParameter("?", false)) > 0;
        }

        // Utility method to hash a password
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
                return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        // Get all non-admin users
        public async Task<List<Client>> GetAllNonAdminUsersAsync()
        {
            string query = "SELECT UserID, Email, IsLocked FROM Users WHERE IsAdmin = False";
            var clients = new List<Client>();

            return await _accessDB.ExecuteAsync(async connection =>
            {
                using (var command = new OleDbCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
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

        // Lock a user account
        public async Task LockUserAccountAsync(int userID)
        {
            string query = "UPDATE Users SET IsLocked = True WHERE UserID = ?";
            await _accessDB.ExecuteNonQueryAsync(query, new OleDbParameter("?", userID));
        }

        // Unlock a user account
        public async Task UnlockUserAccountAsync(int userID)
        {
            string query = "UPDATE Users SET IsLocked = False WHERE UserID = ?";
            await _accessDB.ExecuteNonQueryAsync(query, new OleDbParameter("?", userID));
        }

        // Check if email is registered
        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            return await _accessDB.ExecuteScalarAsync<int>(query, new OleDbParameter("@Email", email)) > 0;
        }

        // Reset user password
        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            string query = "UPDATE Users SET [Password] = @NewPasswordHash WHERE [Email] = @Email";
            string newPasswordHash = HashPassword(newPassword);

            int rowsAffected = await _accessDB.ExecuteNonQueryAsync(query,
                new OleDbParameter("@NewPasswordHash", newPasswordHash),
                new OleDbParameter("@Email", email));

            if (rowsAffected == 0) throw new Exception("No rows were updated. The email may not exist.");

            return true;
        }

        // Log user actions for the audit trail
        public async Task LogUserActionAsync(int userID, string action)
        {
            string query = "INSERT INTO Audit (UserId, [Action], [Timestamp]) VALUES (?, ?, ?)";
            await _accessDB.ExecuteNonQueryAsync(query,
                new OleDbParameter("?", userID),
                new OleDbParameter("?", action),
                new OleDbParameter("?", DateTime.Now));
        }
        public async Task<List<Client>> GetAllUsersIncludingAdminsAsync()
        {
            string query = "SELECT UserID, Email, IsLocked FROM Users";
            var clients = new List<Client>();

            return await _accessDB.ExecuteAsync(async connection =>
            {
                using (var command = new OleDbCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
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

    }
}
