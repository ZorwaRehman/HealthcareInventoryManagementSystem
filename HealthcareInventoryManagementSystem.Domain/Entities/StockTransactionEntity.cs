using System;
using System.Collections.Generic;
using System.Text;


namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class StockTransactionEntity
    {
        // Unique ID for each stock transaction.
        public int StockTransactionId { get; set; }

        // The batch whose quantity changed.
        public int StockBatchId { get; set; }

        // Type of transaction: StockIn, StockOut, or Adjustment.
        public string TransactionType { get; set; } = string.Empty;

        // Quantity added or removed.
        public int Quantity { get; set; }

        // Quantity available before this transaction.
        public int PreviousQuantity { get; set; }

        // Quantity available after this transaction.
        public int NewQuantity { get; set; }

        // Reason for the transaction, if provided.
        public string? Reason { get; set; }

        // Date and time when the transaction occurred.
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        // Navigation property for the related stock batch.
        public StockBatchEntity StockBatch { get; set; } = null!;
    }
}
