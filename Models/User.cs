using System.Collections.Generic;

namespace WarbandOfTheSpiritborn.Models
{
    // Represents a user account in the system
    public class User
    {
        public int Id { get; set; } // Primary key

        public string UserName { get; set; } = string.Empty; // Display name or login name

        public string Password { get; set; } = string.Empty; // Hashed password (never store plain text)

        public string Email { get; set; } = string.Empty; // User's email address

        public bool IsVerified { get; set; } = false; // True if the user has verified their email

        public ICollection<UserRole> UserRoles { get; set; } // Many-to-many relationship to Roles
    }
}
