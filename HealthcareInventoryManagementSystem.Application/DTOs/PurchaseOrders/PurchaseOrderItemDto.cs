
namespace HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders
{
    // Contains individual purchase order item information returned by the API.
    public class PurchaseOrderItemDto
    {
        public int PurchaseOrderItemId { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public int OrderedQuantity { get; set; }

        public int ReceivedQuantity { get; set; }

        // Calculated from ordered quantity and unit price.
        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }

        // Helps Angular display the outstanding quantity.
        public int RemainingQuantity { get; set; }
    }
}
