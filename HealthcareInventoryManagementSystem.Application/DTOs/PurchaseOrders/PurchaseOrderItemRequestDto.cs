
namespace HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders
{
    // Contains the information received for one purchase order item.
    public class PurchaseOrderItemRequestDto
    {
        public int ItemId { get; set; }

        public int OrderedQuantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
