
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    // Defines the operations supported by Purchase Order Management.
    public interface IPurchaseOrderService
    {
        // Get all purchase orders.
        Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync();

        // Get a purchase order by its ID.
        Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(int purchaseOrderId);

        // Create a new draft purchase order without authentication.
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(
            PurchaseOrderRequestDto request);

        // Update an existing draft purchase order.
        Task<PurchaseOrderDto?> UpdatePurchaseOrderAsync(
            int purchaseOrderId,
            PurchaseOrderRequestDto request);

        // Submit a draft purchase order for approval.
        Task<bool> SubmitPurchaseOrderAsync(int purchaseOrderId);

        // Approve a submitted purchase order without authentication.
        Task<bool> ApprovePurchaseOrderAsync(int purchaseOrderId);

        // Mark an approved purchase order as ordered.
        Task<bool> MarkPurchaseOrderAsOrderedAsync(int purchaseOrderId);

        // Cancel an eligible purchase order.
        Task<bool> CancelPurchaseOrderAsync(int purchaseOrderId);
    }
}
