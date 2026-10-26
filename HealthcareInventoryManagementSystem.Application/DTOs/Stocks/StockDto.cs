using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Application.DTOs.Stocks
{
    // Holds stock information for an inventory item.
    public class StockDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }
}
