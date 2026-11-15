
namespace HealthcareInventoryManagementSystem.Application.DTOs.Stocks
{
    // Contains the information required to adjust a stock batch.
    public class StockAdjustmentDto
    {
        public int StockBatchId { get; set; }

        // Expected values: StockIn, StockOut, or Adjustment.
        public string TransactionType { get; set; } = string.Empty;

        // Used for StockIn and StockOut.
        public int Quantity { get; set; }

        // Used for Adjustment: the actual physical quantity counted.
        public int? PhysicalQuantity { get; set; }

        public string? Reason { get; set; }
    }
}
