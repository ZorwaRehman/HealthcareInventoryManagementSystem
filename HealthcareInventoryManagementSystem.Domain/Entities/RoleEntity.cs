using System;
using System.Collections.Generic;
using System.Text;
using HealthcareInventoryManagementSystem.Domain.Entities;

namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class RoleEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Navigation property: one role can belong to many users.
        public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}
