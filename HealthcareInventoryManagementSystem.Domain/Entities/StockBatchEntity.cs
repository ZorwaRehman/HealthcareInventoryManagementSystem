using System;
using System.Collections.Generic;
using System.Text;


namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class StockBatchEntity
    {
        public int StockBatchId { get; set; }

        // Links this batch to its inventory item.
        public int ItemId { get; set; }

        // Unique batch or lot number for the item.
        public string BatchNumber { get; set; } = string.Empty;

        // Expiry date of this batch, if applicable.
        public DateTime? ExpiryDate { get; set; }

        // Available quantity in this batch.
        public int QuantityInStock { get; set; }

        // Purchase cost per unit.
        public decimal UnitPrice { get; set; }

        // Navigation property for the related item.
        public ItemEntity Item { get; set; } = null!;
    }
}

