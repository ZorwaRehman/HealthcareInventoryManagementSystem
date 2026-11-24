namespace HealthcareInventoryManagementSystem.Application.DTOs.Reports
{
    public class ExpiryReportDto
    {
        public int StockBatchId { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string BatchNumber { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public int CurrentQuantity { get; set; }

        public int DaysRemaining { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}