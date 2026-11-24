namespace HealthcareInventoryManagementSystem.Application.DTOs.Reports
{
    public class SupplierReportDto
    {
        public int SupplierId { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? Country { get; set; }

        public int TotalStockBatches { get; set; }

        public int TotalQuantityInStock { get; set; }

        public decimal TotalStockValue { get; set; }

        public bool IsActive { get; set; }
    }
}