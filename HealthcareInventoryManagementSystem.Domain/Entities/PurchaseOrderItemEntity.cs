
namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class PurchaseOrderItemEntity
    {
        public int PurchaseOrderItemId { get; set; }

        // Parent purchase order.
        public int PurchaseOrderId { get; set; }
        public PurchaseOrderEntity PurchaseOrder { get; set; } = null!;

        // Existing inventory item.
        public int ItemId { get; set; }
        public ItemEntity Item { get; set; } = null!;

        // Quantity requested from the supplier.
        public int OrderedQuantity { get; set; }

        // Total quantity received through confirmed receipts.
        public int ReceivedQuantity { get; set; }

        // Price per unit when the order was created.
        public decimal UnitPrice { get; set; }

        // OrderedQuantity multiplied by UnitPrice.
        public decimal LineTotal { get; set; }
    }
}
