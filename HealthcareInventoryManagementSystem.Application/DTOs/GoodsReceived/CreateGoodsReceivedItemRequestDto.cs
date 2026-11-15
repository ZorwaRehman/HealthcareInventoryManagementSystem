
using System;

namespace HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived
{
    // Defines one delivered item or batch within a Goods Received document.
    public class CreateGoodsReceivedItemRequestDto
    {
        // Identifies the item line from the selected Purchase Order.
        public int PurchaseOrderItemId { get; set; }

        // Supplier's batch or lot number.
        public string BatchNumber { get; set; } = string.Empty;

        // Total quantity delivered by the supplier.
        public int DeliveredQuantity { get; set; }

        // Quantity accepted and added to available stock.
        public int AcceptedQuantity { get; set; }

        // Quantity rejected because it does not meet requirements.
        public int RejectedQuantity { get; set; }

        // Quantity held for inspection before acceptance.
        public int QuarantinedQuantity { get; set; }

        // Optional manufacturing date of this batch.
        public DateTime? ManufactureDate { get; set; }

        // Optional expiry date of this batch.
        public DateTime? ExpiryDate { get; set; }

        // Optional location where the batch will be stored.
        public string? StorageLocation { get; set; }
    }
}
