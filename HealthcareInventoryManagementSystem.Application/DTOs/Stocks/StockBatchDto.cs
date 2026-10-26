using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Application.DTOs.Stocks
{
    // Holds details for one stock batch.
    public class StockBatchDto
    {
        public int StockBatchId { get; set; }

        public int ItemId { get; set; }

        public string BatchNumber { get; set; } = string.Empty;

        public int QuantityInStock { get; set; }

        public decimal UnitPrice { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
