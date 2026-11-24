
namespace HealthcareInventoryManagementSystem.Application.DTOs.Reports
{
    public class StockReportDto
    {
        public int ItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public int TotalQuantity { get; set; }

        public int ReorderLevel { get; set; }

        public decimal StockValue { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
