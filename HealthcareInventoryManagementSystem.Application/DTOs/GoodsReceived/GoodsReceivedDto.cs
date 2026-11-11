
using System;
using System.Collections.Generic;
using HealthcareInventoryManagementSystem.Domain.Enums;

namespace HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived
{
    // Represents a complete Goods Received document in API responses.
    public class GoodsReceivedDto
    {
        public int GoodsReceivedId { get; set; }

        public string ReceiptNumber { get; set; } = string.Empty;

        public int PurchaseOrderId { get; set; }

        public string PurchaseOrderNumber { get; set; } = string.Empty;

        public string SupplierName { get; set; } = string.Empty;

        public DateTime ReceivedDate { get; set; }

        public string? DeliveryNoteNumber { get; set; }

        public string? Notes { get; set; }

        public GoodsReceivedStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        // Contains all item batches recorded in this receipt.
        public List<GoodsReceivedItemDto> Items { get; set; } = new();
    }
}
