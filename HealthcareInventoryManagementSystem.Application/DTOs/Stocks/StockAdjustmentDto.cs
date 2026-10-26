using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Application.DTOs.Stocks
{
    // Contains the information required to adjust a stock batch.
    public class StockAdjustmentDto
    {
        public int StockBatchId { get; set; }

        // Expected values: StockIn, StockOut, or Adjustment.
        public string TransactionType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string? Reason { get; set; }
    }
}
