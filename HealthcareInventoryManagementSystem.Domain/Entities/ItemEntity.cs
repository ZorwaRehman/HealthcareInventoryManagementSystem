using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class ItemEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Category { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public int ReorderLevel { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}

