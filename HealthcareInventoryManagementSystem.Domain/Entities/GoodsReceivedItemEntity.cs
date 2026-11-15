
using System;

namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class GoodsReceivedItemEntity
    {
        public int GoodsReceivedItemId { get; set; }

        // Links this detail to its Goods Received document.
        public int GoodsReceivedId { get; set; }

        public GoodsReceivedEntity GoodsReceived { get; set; } = null!;

        // Links to the exact item line on the Purchase Order.
        public int PurchaseOrderItemId { get; set; }

        public PurchaseOrderItemEntity PurchaseOrderItem { get; set; } = null!;

        // Identifies the supplier's delivered batch or lot.
        public string BatchNumber { get; set; } = string.Empty;

        // Total quantity physically delivered.
        public int DeliveredQuantity { get; set; }

        // Quantity accepted and added to available inventory.
        public int AcceptedQuantity { get; set; }

        // Quantity rejected during inspection.
        public int RejectedQuantity { get; set; }

        // Quantity awaiting inspection.
        public int QuarantinedQuantity { get; set; }

        // Optional manufacturing and expiry information.
        public DateTime? ManufactureDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        // Optional physical storage location.
        public string? StorageLocation { get; set; }

        // Links to the stock batch after accepted goods are processed.
        public int? StockBatchId { get; set; }

        public StockBatchEntity? StockBatch { get; set; }
    }
}
