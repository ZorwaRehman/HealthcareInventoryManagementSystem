
using System;
using System.Collections.Generic;

namespace HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders
{
    // Contains purchase order information returned by the API.
    public class PurchaseOrderDto
    {
        public int PurchaseOrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int SupplierId { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        // Returns a readable status instead of an integer.
        public string Status { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Notes { get; set; }

        // Date when the purchase order was created.
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Date when the purchase order was approved.
        public DateTime? ApprovedAt { get; set; }

        // Contains all items belonging to this purchase order.
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }
}
