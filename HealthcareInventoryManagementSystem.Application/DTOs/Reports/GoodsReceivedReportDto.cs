using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Application.DTOs.Reports
{
    public class GoodsReceivedReportDto
    {
        public int GoodsReceivedId { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public int PurchaseOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime ReceivedDate { get; set; }
        public string? DeliveryNoteNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public int TotalDeliveredQuantity { get; set; }
        public int TotalAcceptedQuantity { get; set; }
        public int TotalRejectedQuantity { get; set; }
        public int TotalQuarantinedQuantity { get; set; }
        public string? Notes { get; set; }
    }
}