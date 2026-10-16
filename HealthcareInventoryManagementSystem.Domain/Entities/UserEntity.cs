using System;
using HealthcareInventoryManagementSystem.Domain.Entities;

namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // Store a secure password hash, never the plain-text password.
        public string PasswordHash { get; set; } = string.Empty;

        public int RoleId { get; set; }

        // Navigation property for the user's role.
        public RoleEntity? Role { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
