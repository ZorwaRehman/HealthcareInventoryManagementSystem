
using System;
using System.Collections.Generic;
using HealthcareInventoryManagementSystem.Domain.Enums;

namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class GoodsReceivedEntity
    {
        public int GoodsReceivedId { get; set; }

        // Unique reference number for this goods receipt.
        public string ReceiptNumber { get; set; } = string.Empty;

        // Every receipt must belong to an existing Purchase Order.
        public int PurchaseOrderId { get; set; }

        public PurchaseOrderEntity PurchaseOrder { get; set; } = null!;

        // Date on which the goods physically arrived.
        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;

        // Optional supplier delivery note reference.
        public string? DeliveryNoteNumber { get; set; }

        public string? Notes { get; set; }

        // Tracks whether the receipt is a draft, confirmed, or cancelled.
        public GoodsReceivedStatus Status { get; set; }
            = GoodsReceivedStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ConfirmedAt { get; set; }

        // Items delivered under this receipt.
        public ICollection<GoodsReceivedItemEntity> Items { get; set; }
            = new List<GoodsReceivedItemEntity>();
    }
}
