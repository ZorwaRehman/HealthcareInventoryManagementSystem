
using HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived;
using HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders;

namespace HealthcareInventoryManagementSystem.Application.Interfaces
{
    public interface IGoodsReceivedService
    {
        // Retrieves all Goods Received documents.
        Task<IEnumerable<GoodsReceivedDto>> GetAllAsync();

        // Retrieves one Goods Received document by its ID.
        Task<GoodsReceivedDto?> GetByIdAsync(int id);

        // Retrieves Purchase Order details for receiving.
        Task<PurchaseOrderDto?> GetPurchaseOrderForReceivingAsync(int purchaseOrderId);

        // Creates a new Goods Received document in Draft status.
        Task<GoodsReceivedDto> CreateAsync(CreateGoodsReceivedRequestDto request);

        // Confirms the delivery and updates accepted stock quantities.
        Task<GoodsReceivedDto> ConfirmAsync(int id);

        // Cancels a document that is still in Draft status.
        Task CancelAsync(int id);

        // Releases quarantined quantities after successful inspection.
        Task<GoodsReceivedDto> ReleaseQuarantinedQuantityAsync(
            int goodsReceivedItemId,
            int quantity);
    }
}
