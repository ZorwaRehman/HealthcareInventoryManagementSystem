using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Application.DTOs.Stocks
{
    // Represents one recorded stock transaction.
    public class StockTransactionDto
    {
        public int StockTransactionId { get; set; }

        public int StockBatchId { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public int PreviousQuantity { get; set; }

        public int NewQuantity { get; set; }

        public string? Reason { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
