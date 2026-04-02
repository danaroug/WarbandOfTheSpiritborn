using System;
using WarbandOfTheSpiritborn.Models;

namespace WarbandOfTheSpiritborn.Models
{
    // Represents a temporary verification token tied to a user
    public class EmailVerification
    {
        public int Id { get; set; } // Primary key

        public string Token { get; set; } = string.Empty; // Random token (e.g., GUID) sent via email

        public DateTime Expiry { get; set; } // Time until token becomes invalid

        public int UserId { get; set; } // Foreign key linking to the User
    }
}

