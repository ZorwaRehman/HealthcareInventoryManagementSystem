
namespace HealthcareInventoryManagementSystem.Application.DTOs.Reports
{
    public class PurchaseReportDto
    {
        public int PurchaseOrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int SupplierId { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public int ItemCount { get; set; }

        public int TotalOrderedQuantity { get; set; }

        public int TotalReceivedQuantity { get; set; }

        public int TotalRemainingQuantity { get; set; }

        public decimal Subtotal { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Notes { get; set; }
    }
}
