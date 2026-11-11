
using System;
using System.Collections.Generic;

namespace HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived
{
    // Defines the data required to create a Goods Received document.
    public class CreateGoodsReceivedRequestDto
    {
        // The Purchase Order against which goods are being received.
        public int PurchaseOrderId { get; set; }

        // Date on which the supplier delivered the goods.
        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;

        // Optional reference number from the supplier's delivery note.
        public string? DeliveryNoteNumber { get; set; }

        // Optional additional information about this delivery.
        public string? Notes { get; set; }

        // Individual items and their delivery quantities.
        public List<CreateGoodsReceivedItemRequestDto> Items { get; set; } = new();
    }
}
