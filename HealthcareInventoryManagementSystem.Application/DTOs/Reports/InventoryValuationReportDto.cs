namespace HealthcareInventoryManagementSystem.Application.DTOs.Reports
{
    public class InventoryValuationReportDto
    {
        public int ItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public int StockBatchId { get; set; }

        public string BatchNumber { get; set; } = string.Empty;

        public DateTime? ExpiryDate { get; set; }

        public int QuantityInStock { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalValue { get; set; }
    }
}