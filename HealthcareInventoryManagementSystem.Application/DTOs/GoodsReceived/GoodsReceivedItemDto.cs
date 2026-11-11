
using System;

namespace HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived
{
    // Represents one received item in the API response.
    public class GoodsReceivedItemDto
    {
        public int GoodsReceivedItemId { get; set; }

        public int PurchaseOrderItemId { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string BatchNumber { get; set; } = string.Empty;

        public int DeliveredQuantity { get; set; }

        public int AcceptedQuantity { get; set; }

        public int RejectedQuantity { get; set; }

        public int QuarantinedQuantity { get; set; }

        public DateTime? ManufactureDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string? StorageLocation { get; set; }

        public int? StockBatchId { get; set; }
    }
}
