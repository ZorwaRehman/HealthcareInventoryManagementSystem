using System;
using System.Collections.Generic;

namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? TaxNumber { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property for stock batches supplied by this supplier.
        public ICollection<StockBatchEntity> StockBatches { get; set; }
            = new List<StockBatchEntity>();
    }
}
