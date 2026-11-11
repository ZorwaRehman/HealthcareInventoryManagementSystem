
using System;
using System.Collections.Generic;

namespace HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders
{
    // Contains the information received when creating or updating a purchase order.
    public class PurchaseOrderRequestDto
    {
        public int SupplierId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpectedDeliveryDate { get; set; }

        // Tax percentage; the backend calculates the actual tax amount.
        public decimal TaxRate { get; set; }

        public string? Notes { get; set; }

        // A purchase order can contain multiple items.
        public List<PurchaseOrderItemRequestDto> Items { get; set; } = new();
    }
}
