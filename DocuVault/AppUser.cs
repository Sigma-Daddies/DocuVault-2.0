using System;

namespace DocuVault
{
    public class AppUser
    {
        public string Email { get; set; }  // Email of the client
        public bool IsAdministrator { get; set; }  // Whether the client is an admin or not

        // Constructor to initialize client properties
        public AppUser(string email, bool isAdministrator)
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
        public AppUser() { }

        // Optional: Override the ToString() method to return a custom string for the client
        public override string ToString()
        {
            return $"Email: {Email}, Admin: {IsAdministrator}";
        }

        // Method to compare two AppUser objects for equality based on their Email and IsAdministrator status
        public override bool Equals(object obj)
        {
            if (obj is AppUser otherUser)
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
    }
}
