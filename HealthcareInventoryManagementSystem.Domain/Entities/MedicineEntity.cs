using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class MedicineEntity
    {
        public int MedicineId { get; set; }

        // Links this medicine to its common inventory item.
        public int ItemId { get; set; }

        // Medicine-specific information.
        public string GenericName { get; set; } = string.Empty;

        // Navigation property for the related inventory item.
        public ItemEntity Item { get; set; } = null!;
    }
}
