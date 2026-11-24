
namespace HealthcareInventoryManagementSystem.Application.DTOs.Reports
{
    public class StockTransactionReportDto
    {
        public int StockTransactionId { get; set; }

        public int StockBatchId { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string BatchNumber { get; set; } = string.Empty;

        public string TransactionType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public int PreviousQuantity { get; set; }

        public int NewQuantity { get; set; }

        public string? Reason { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
