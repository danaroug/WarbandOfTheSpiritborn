using System.Collections.Generic;

namespace WarbandOfTheSpiritborn.Models
{
    // Represents a user role (e.g., admin, user, guest)
    public class Role
    {
        public int Id { get; set; } // Primary key

        public string Name { get; set; } = string.Empty; // Role name ("admin", "user", etc.)

        public ICollection<UserRole> UserRoles { get; set; } // Users assigned to this role
    }
}

