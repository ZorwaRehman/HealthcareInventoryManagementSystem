using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class Medicine
    {
        public int MedicineId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string GenericName { get; set; } = string.Empty;

        public string BatchNumber { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public int QuantityInStock { get; set; }

        public decimal UnitPrice { get; set; }

        public int ReorderLevel { get; set; }
    }
}
