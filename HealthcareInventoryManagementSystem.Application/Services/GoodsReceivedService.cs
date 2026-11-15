using AutoMapper;
using FluentValidation;
using HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived;
using HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders;
using HealthcareInventoryManagementSystem.Application.Interfaces;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Entities;
using HealthcareInventoryManagementSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace HealthcareInventoryManagementSystem.Application.Services
{
    public class GoodsReceivedService(
        IGenericRepository<GoodsReceivedEntity> goodsReceivedRepository,
        IGenericRepository<GoodsReceivedItemEntity> goodsReceivedItemRepository,
        IGenericRepository<PurchaseOrderEntity> purchaseOrderRepository,
        IGenericRepository<PurchaseOrderItemEntity> purchaseOrderItemRepository,
        IGenericRepository<StockBatchEntity> stockBatchRepository,
        IGenericRepository<StockTransactionEntity> stockTransactionRepository,
        IStockTransactionManager transactionManager,
        IMapper mapper,
        IValidator<CreateGoodsReceivedRequestDto> validator,
        ILogger<GoodsReceivedService> logger
    ) : IGoodsReceivedService
    {
        // ---------------------------------------------------------
        // 1. GET ALL GOODS RECEIVED DOCUMENTS
        // ---------------------------------------------------------

        public async Task<IEnumerable<GoodsReceivedDto>> GetAllAsync()
        {
            var receipts =
                await goodsReceivedRepository.FindAllWithNestedIncludesAsync(
                    receipt => true,
                    "PurchaseOrder.Supplier",
                    "Items.PurchaseOrderItem.Item",
                    "Items.StockBatch");

            return receipts
                .OrderByDescending(receipt => receipt.ReceivedDate)
                .Select(receipt => mapper.Map<GoodsReceivedDto>(receipt));
        }
        // 2. GET GOODS RECEIVED DOCUMENT BY ID
        public async Task<GoodsReceivedDto?> GetByIdAsync(int id)
        {
            var receipt = await GetReceiptEntityAsync(id);

            if (receipt == null)
                return null;

            return mapper.Map<GoodsReceivedDto>(receipt);
        }

        // 3. GET PURCHASE ORDER FOR RECEIVING
        public async Task<PurchaseOrderDto?> GetPurchaseOrderForReceivingAsync(
            int purchaseOrderId)
        {
            var purchaseOrders =
                await purchaseOrderRepository.FindAllWithNestedIncludesAsync(
                    po => po.PurchaseOrderId == purchaseOrderId,
                    "Supplier",
                    "Items.Item");

            var purchaseOrder = purchaseOrders.FirstOrDefault();

            if (purchaseOrder == null)
                return null;

            // Goods can only be received against an ordered
            // or partially received Purchase Order.
            if (purchaseOrder.Status != PurchaseOrderStatus.Ordered &&
                purchaseOrder.Status != PurchaseOrderStatus.PartiallyReceived)
            {
                throw new InvalidOperationException(
                    "Goods can only be received against an Ordered or Partially Received Purchase Order.");
            }

            // Reuse the existing Purchase Order response DTO.
            return mapper.Map<PurchaseOrderDto>(purchaseOrder);
        }

        // 4. CREATE GOODS RECEIVED DOCUMENT
        public async Task<GoodsReceivedDto> CreateAsync(
            CreateGoodsReceivedRequestDto request)
        {
            // Validate the incoming request.
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Find the Purchase Order.
            var purchaseOrder =
                await purchaseOrderRepository.GetByIdAsync(request.PurchaseOrderId);

            if (purchaseOrder == null)
                throw new KeyNotFoundException("Purchase Order was not found.");

            // Check Purchase Order status.
            if (purchaseOrder.Status != PurchaseOrderStatus.Ordered &&
                purchaseOrder.Status != PurchaseOrderStatus.PartiallyReceived)
            {
                throw new InvalidOperationException(
                    "Goods can only be received against an Ordered or Partially Received Purchase Order.");
            }

            // Load all items belonging to this Purchase Order.
            var purchaseOrderItems =
                await purchaseOrderItemRepository.FindAllAsync(
                    item => item.PurchaseOrderId == request.PurchaseOrderId,
                    item => item.Item);

            var purchaseOrderItemsDictionary =
                purchaseOrderItems.ToDictionary(item => item.PurchaseOrderItemId);

            // Check that every submitted line belongs to this Purchase Order.
            foreach (var requestItem in request.Items)
            {
                if (!purchaseOrderItemsDictionary.ContainsKey(
                    requestItem.PurchaseOrderItemId))
                {
                    throw new InvalidOperationException(
                        $"Purchase Order item {requestItem.PurchaseOrderItemId} does not belong to the selected Purchase Order.");
                }
            }

            // Create the Goods Received document.
            var receipt = new GoodsReceivedEntity
            {
                // GUID-based number avoids relying on timestamps for uniqueness.
                ReceiptNumber = $"GRN-{Guid.NewGuid():N}",
                PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                ReceivedDate = request.ReceivedDate,
                DeliveryNoteNumber = request.DeliveryNoteNumber,
                Notes = request.Notes,
                Status = GoodsReceivedStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            // Create the individual delivery lines.
            foreach (var requestItem in request.Items)
            {
                var receiptItem = new GoodsReceivedItemEntity
                {
                    PurchaseOrderItemId = requestItem.PurchaseOrderItemId,
                    BatchNumber = requestItem.BatchNumber.Trim(),
                    DeliveredQuantity = requestItem.DeliveredQuantity,
                    AcceptedQuantity = requestItem.AcceptedQuantity,
                    RejectedQuantity = requestItem.RejectedQuantity,
                    QuarantinedQuantity = requestItem.QuarantinedQuantity,
                    ManufactureDate = requestItem.ManufactureDate,
                    ExpiryDate = requestItem.ExpiryDate,
                    StorageLocation = requestItem.StorageLocation
                };

                receipt.Items.Add(receiptItem);
            }

            // Save the receipt and its item lines atomically.
            await transactionManager.ExecuteInTransactionAsync(async () =>
            {
                await goodsReceivedRepository.AddAsync(receipt);
            });

            logger.LogInformation(
                "Goods Received document {ReceiptNumber} created for Purchase Order {PurchaseOrderId}.",
                receipt.ReceiptNumber,
                receipt.PurchaseOrderId);

            return await GetByIdAsync(receipt.GoodsReceivedId)
                ?? throw new InvalidOperationException(
                    "Goods Received document was created but could not be retrieved.");
        }

        // 5. CONFIRM GOODS RECEIVED DOCUMENT
        public async Task<GoodsReceivedDto> ConfirmAsync(int id)
        {
            var receipt = await GetReceiptEntityAsync(id);

            if (receipt == null)
                throw new KeyNotFoundException("Goods Received document was not found.");

            if (receipt.Status != GoodsReceivedStatus.Draft)
                throw new InvalidOperationException(
                    "Only Draft Goods Received documents can be confirmed.");

            var purchaseOrder = receipt.PurchaseOrder;

            if (purchaseOrder.Status != PurchaseOrderStatus.Ordered &&
                purchaseOrder.Status != PurchaseOrderStatus.PartiallyReceived)
            {
                throw new InvalidOperationException(
                    "The Purchase Order is not available for receiving.");
            }

            // Load tracked Purchase Order items.
            var poItems = await purchaseOrderItemRepository.FindAllAsync(
                item => item.PurchaseOrderId == receipt.PurchaseOrderId);

            var poItemsDictionary = poItems.ToDictionary(
                item => item.PurchaseOrderItemId);

            // Calculate the accepted quantity for each Purchase Order line.
            var acceptedQuantities = receipt.Items
                .GroupBy(item => item.PurchaseOrderItemId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(item => item.AcceptedQuantity));

            // Verify that accepted quantities do not exceed
            // the outstanding Purchase Order quantities.
            foreach (var accepted in acceptedQuantities)
            {
                if (!poItemsDictionary.TryGetValue(
                    accepted.Key, out var poItem))
                {
                    throw new InvalidOperationException(
                        "A received item does not belong to this Purchase Order.");
                }

                var remainingQuantity =
                    poItem.OrderedQuantity - poItem.ReceivedQuantity;

                if (accepted.Value > remainingQuantity)
                {
                    throw new InvalidOperationException(
                        $"Accepted quantity exceeds the remaining quantity for Purchase Order item {poItem.PurchaseOrderItemId}.");
                }
            }

            await transactionManager.ExecuteInTransactionAsync(async () =>
            {
                // Add only accepted quantities to stock.
                foreach (var receiptItem in receipt.Items)
                {
                    if (receiptItem.AcceptedQuantity <= 0)
                        continue;

                    var poItem =
                        poItemsDictionary[receiptItem.PurchaseOrderItemId];

                    await AddAcceptedStockAsync(
                        receiptItem,
                        purchaseOrder,
                        poItem,
                        receiptItem.AcceptedQuantity,
                        $"Goods Received {receipt.ReceiptNumber}");
                }

                // Update Purchase Order accepted quantities.
                foreach (var accepted in acceptedQuantities)
                {
                    var poItem = poItemsDictionary[accepted.Key];

                    poItem.ReceivedQuantity += accepted.Value;
                }

                // Update the Purchase Order status.
                await UpdatePurchaseOrderStatusAsync(purchaseOrder);

                // Confirm the Goods Received document.
                receipt.Status = GoodsReceivedStatus.Confirmed;
                receipt.ConfirmedAt = DateTime.UtcNow;
            });

            logger.LogInformation(
                "Goods Received document {ReceiptNumber} confirmed.",
                receipt.ReceiptNumber);

            return await GetByIdAsync(id)
                ?? throw new InvalidOperationException(
                    "Confirmed Goods Received document could not be retrieved.");
        }

        // 6. CANCEL GOODS RECEIVED DOCUMENT
    
        public async Task CancelAsync(int id)
        {
            var receipt = await goodsReceivedRepository.GetByIdAsync(id);

            if (receipt == null)
                throw new KeyNotFoundException("Goods Received document was not found.");

            if (receipt.Status != GoodsReceivedStatus.Draft)
            {
                throw new InvalidOperationException(
                    "Only Draft Goods Received documents can be cancelled.");
            }

            await transactionManager.ExecuteInTransactionAsync(async () =>
            {
                receipt.Status = GoodsReceivedStatus.Cancelled;
            });

            logger.LogInformation(
                "Goods Received document {ReceiptNumber} cancelled.",
                receipt.ReceiptNumber);
        }

        // 7. RELEASE QUARANTINED QUANTITY
        public async Task<GoodsReceivedDto> ReleaseQuarantinedQuantityAsync(
            int goodsReceivedItemId,
            int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException(
                    "Release quantity must be greater than zero.");

            var receiptItems =
                await goodsReceivedItemRepository.FindAllWithNestedIncludesAsync(
                    item => item.GoodsReceivedItemId == goodsReceivedItemId,
                    "GoodsReceived.PurchaseOrder",
                    "PurchaseOrderItem.Item",
                    "StockBatch");

            var receiptItem = receiptItems.FirstOrDefault();

            if (receiptItem == null)
                throw new KeyNotFoundException(
                    "Goods Received item was not found.");

            if (receiptItem.GoodsReceived.Status != GoodsReceivedStatus.Confirmed)
            {
                throw new InvalidOperationException(
                    "Quarantined quantities can only be released from a confirmed receipt.");
            }

            if (quantity > receiptItem.QuarantinedQuantity)
            {
                throw new InvalidOperationException(
                    "Release quantity exceeds the quarantined quantity.");
            }

            var purchaseOrder = receiptItem.GoodsReceived.PurchaseOrder;
            var poItem = receiptItem.PurchaseOrderItem;

            if (poItem.PurchaseOrderId != purchaseOrder.PurchaseOrderId)
            {
                throw new InvalidOperationException(
                    "The received item does not belong to the receipt's Purchase Order.");
            }

            var remainingQuantity =
                poItem.OrderedQuantity - poItem.ReceivedQuantity;

            if (quantity > remainingQuantity)
            {
                throw new InvalidOperationException(
                    "Release quantity exceeds the remaining Purchase Order quantity.");
            }

            await transactionManager.ExecuteInTransactionAsync(async () =>
            {
                // Add the released quantity to available stock.
                await AddAcceptedStockAsync(
                    receiptItem,
                    purchaseOrder,
                    poItem,
                    quantity,
                    $"Quarantine release - {receiptItem.GoodsReceived.ReceiptNumber}");

                // Move quantity from quarantine to accepted.
                receiptItem.QuarantinedQuantity -= quantity;
                receiptItem.AcceptedQuantity += quantity;

                // Update the accepted Purchase Order quantity.
                poItem.ReceivedQuantity += quantity;

                // Update Purchase Order status.
                await UpdatePurchaseOrderStatusAsync(purchaseOrder);
            });

            logger.LogInformation(
                "{Quantity} quarantined units released for Goods Received item {ItemId}.",
                quantity,
                goodsReceivedItemId);

            return await GetByIdAsync(receiptItem.GoodsReceivedId)
                ?? throw new InvalidOperationException(
                    "Goods Received document could not be retrieved after quarantine release.");
        }
        // PRIVATE HELPER: LOAD RECEIPT WITH RELATED ENTITIES
        private async Task<GoodsReceivedEntity?> GetReceiptEntityAsync(int id)
        {
            var receipts =
                await goodsReceivedRepository.FindAllWithNestedIncludesAsync(
                    receipt => receipt.GoodsReceivedId == id,
                    "PurchaseOrder.Supplier",
                    "PurchaseOrder.Items",
                    "Items.PurchaseOrderItem.Item",
                    "Items.StockBatch");

            return receipts.FirstOrDefault();
        }
        // PRIVATE HELPER: ADD ACCEPTED QUANTITY TO STOCK
        private async Task AddAcceptedStockAsync(
            GoodsReceivedItemEntity receiptItem,
            PurchaseOrderEntity purchaseOrder,
            PurchaseOrderItemEntity poItem,
            int quantity,
            string reason)
        {
            if (quantity <= 0)
                return;

            // Find an existing batch using the existing unique
            // ItemId + BatchNumber combination.
            var stockBatch = await stockBatchRepository.FindAsync(
                batch =>
                    batch.ItemId == poItem.ItemId &&
                    batch.BatchNumber == receiptItem.BatchNumber);

            if (stockBatch == null)
            {
                // Create a new stock batch if one does not exist.
                stockBatch = new StockBatchEntity
                {
                    ItemId = poItem.ItemId,
                    SupplierId = purchaseOrder.SupplierId,
                    BatchNumber = receiptItem.BatchNumber,
                    ExpiryDate = receiptItem.ExpiryDate,
                    QuantityInStock = quantity,
                    UnitPrice = poItem.UnitPrice
                };

                await stockBatchRepository.AddAsync(stockBatch);
            }
            else
            {
                // Do not combine batches with conflicting supplier
                // or unit-price information.
                if (stockBatch.SupplierId.HasValue &&
                    stockBatch.SupplierId.Value != purchaseOrder.SupplierId)
                {
                    throw new InvalidOperationException(
                        "An existing stock batch has a different supplier.");
                }

                if (stockBatch.UnitPrice != poItem.UnitPrice)
                {
                    throw new InvalidOperationException(
                        "An existing stock batch has a different unit price.");
                }

                if (!stockBatch.SupplierId.HasValue)
                    stockBatch.SupplierId = purchaseOrder.SupplierId;

                stockBatch.QuantityInStock += quantity;

                if (receiptItem.ExpiryDate.HasValue)
                    stockBatch.ExpiryDate = receiptItem.ExpiryDate;
            }

            // Link the receipt line to its stock batch.
            // Assigning the navigation property also works
            // when the stock batch is newly created.
            receiptItem.StockBatch = stockBatch;

            // Record the stock-in transaction.
            var previousQuantity =
                stockBatch.QuantityInStock - quantity;

            var stockTransaction = new StockTransactionEntity
            {
                StockBatch = stockBatch,
                StockBatchId = stockBatch.StockBatchId,
                TransactionType = "StockIn",
                Quantity = quantity,
                PreviousQuantity = previousQuantity,
                NewQuantity = stockBatch.QuantityInStock,
                Reason = reason,
                TransactionDate = DateTime.UtcNow
            };

            await stockTransactionRepository.AddAsync(stockTransaction);
        }
        // PRIVATE HELPER: UPDATE PURCHASE ORDER STATUS
        private async Task UpdatePurchaseOrderStatusAsync(
            PurchaseOrderEntity purchaseOrder)
        {
            var items = await purchaseOrderItemRepository.FindAllAsync(
                item => item.PurchaseOrderId == purchaseOrder.PurchaseOrderId);

            if (!items.Any())
                return;

            // All ordered quantities have been accepted.
            if (items.All(item =>
                    item.ReceivedQuantity >= item.OrderedQuantity))
            {
                purchaseOrder.Status = PurchaseOrderStatus.Received;
            }
            // At least one quantity has been accepted.
            else if (items.Any(item => item.ReceivedQuantity > 0))
            {
                purchaseOrder.Status = PurchaseOrderStatus.PartiallyReceived;
            }
            else
            {
                purchaseOrder.Status = PurchaseOrderStatus.Ordered;
            }
        }
    }
}