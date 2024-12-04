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
        public string Email { get; set; }  // Email of the client
        public bool IsAdministrator { get; set; }  // Whether the client is an admin or not

        public UserService(AccessDB accessDB)
        {
            _accessDB = accessDB ?? throw new ArgumentNullException(nameof(accessDB));
        }

        // Constructor to initialize client properties
        public UserService(string email, bool isAdministrator)
        {
            // Ensure the email is not null or empty
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }

            Email = email;
            IsAdministrator = isAdministrator;
        }

        // Parameterless constructor for deserialization or default initialization
        public UserService() { }

        // Optional: Override the ToString() method to return a custom string for the client
        public override string ToString()
        {
            return $"Email: {Email}, Admin: {IsAdministrator}";
        }

        // Method to compare two AppUser objects for equality based on their Email and IsAdministrator status
        public override bool Equals(object obj)
        {
            if (obj is UserService otherUser)
            {
                return this.Email == otherUser.Email && this.IsAdministrator == otherUser.IsAdministrator;
            }
            return false;
        }

        // Override GetHashCode to ensure proper hash calculation if Equals() is overridden
        public override int GetHashCode()
        {
            return Email.GetHashCode() ^ IsAdministrator.GetHashCode();
        }

        public bool IsAuthenticated(string email, string password)
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
        public UserService GetUserFromDatabase(string email)
        {
            return _accessDB.Execute(connection =>
            {
                string query = "SELECT Email, IsAdmin FROM Users WHERE Email = @Email";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool isAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"));
                            return new UserService(email, isAdmin);
                        }
                        return null;
                    }
                }
            });
        }
        public async Task<bool> RegisterUserAsync(string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                throw new ArgumentException("All fields are required.");
            }
            if (password != confirmPassword)
            {
                throw new ArgumentException("Passwords do not match.");
            }
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
                        return rowsAffected > 0;
                    }
                });
            });
        }
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
