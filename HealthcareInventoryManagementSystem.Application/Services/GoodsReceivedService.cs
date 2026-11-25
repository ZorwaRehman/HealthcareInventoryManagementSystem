
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
using System.Globalization;
using System.Linq.Expressions;
using HealthcareInventoryManagementSystem.Application.Dtos.Pagination;

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
        // 1. GET ALL GOODS RECEIVED DOCUMENTS
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

        // Retrieves paginated Goods Received documents with optional search and status filters.
        public async Task<PaginationResponseDto<GoodsReceivedDto>> GetPagedAsync(
            PaginationRequestDto request,
            string? search = null,
            GoodsReceivedStatus? status = null)
        {
            var pageNumber = Math.Max(1, request.PageNumber);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            var searchTerm = string.IsNullOrWhiteSpace(search)
                ? null
                : search.Trim();

            // Apply search and status filters at database level.
            Expression<Func<GoodsReceivedEntity, bool>> predicate = receipt =>
                (!status.HasValue || receipt.Status == status.Value)
                &&
                (
                    searchTerm == null
                    || receipt.ReceiptNumber.Contains(searchTerm)
                    || (receipt.DeliveryNoteNumber != null
                        && receipt.DeliveryNoteNumber.Contains(searchTerm))
                    || (receipt.PurchaseOrder != null
                        && receipt.PurchaseOrder.OrderNumber.Contains(searchTerm))
                    || (receipt.PurchaseOrder != null
                        && receipt.PurchaseOrder.Supplier != null
                        && receipt.PurchaseOrder.Supplier.Name.Contains(searchTerm))
                );

            // Retrieve only the requested page.
            var pagedResult = await goodsReceivedRepository.GetPagedAsync(
                predicate,
                receipt => receipt.ReceivedDate,
                pageNumber,
                pageSize,
                descending: true);

            // Get IDs of the receipts on this page.
            var receiptIds = pagedResult.Items
                .Select(receipt => receipt.GoodsReceivedId)
                .ToList();

            // Load related details only for the current page.
            var detailedReceipts = receiptIds.Count == 0
                ? new List<GoodsReceivedEntity>()
                : (await goodsReceivedRepository.FindAllWithNestedIncludesAsync(
                    receipt => receiptIds.Contains(receipt.GoodsReceivedId),
                    "PurchaseOrder.Supplier",
                    "Items.PurchaseOrderItem.Item",
                    "Items.StockBatch")).ToList();

            // Restore the original pagination order.
            var receiptsById = detailedReceipts.ToDictionary(
                receipt => receipt.GoodsReceivedId);

            var receiptDtos = receiptIds
                .Where(id => receiptsById.ContainsKey(id))
                .Select(id => mapper.Map<GoodsReceivedDto>(receiptsById[id]))
                .ToList();

            return new PaginationResponseDto<GoodsReceivedDto>
            {
                Items = receiptDtos,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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

            if (purchaseOrder.Status != PurchaseOrderStatus.Ordered &&
                purchaseOrder.Status != PurchaseOrderStatus.PartiallyReceived)
            {
                throw new InvalidOperationException(
                    "Goods can only be received against an Ordered or Partially Received Purchase Order.");
            }

            return mapper.Map<PurchaseOrderDto>(purchaseOrder);
        }

        // 4. CREATE GOODS RECEIVED DOCUMENT
        public async Task<GoodsReceivedDto> CreateAsync(
            CreateGoodsReceivedRequestDto request)
        {
            // Validate request.
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Find Purchase Order.
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

            // Load Purchase Order items.
            var purchaseOrderItems =
                await purchaseOrderItemRepository.FindAllAsync(
                    item => item.PurchaseOrderId == request.PurchaseOrderId,
                    item => item.Item);

            var purchaseOrderItemsDictionary =
                purchaseOrderItems.ToDictionary(item => item.PurchaseOrderItemId);

            // Verify submitted items belong to the selected Purchase Order.
            foreach (var requestItem in request.Items)
            {
                if (!purchaseOrderItemsDictionary.ContainsKey(
                    requestItem.PurchaseOrderItemId))
                {
                    throw new InvalidOperationException(
                        $"Purchase Order item {requestItem.PurchaseOrderItemId} does not belong to the selected Purchase Order.");
                }
            }

            // Generate the daily-sequence receipt number.
            var createdAt = DateTime.UtcNow;

            var receiptNumber =
                await GenerateReceiptNumberAsync(createdAt);

            // Log the generated number for debugging.
            logger.LogWarning(
                "NEW RECEIPT NUMBER GENERATED: {ReceiptNumber}",
                receiptNumber);

            // Create the Goods Received document.
            var receipt = new GoodsReceivedEntity
            {
                ReceiptNumber = receiptNumber,
                PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                ReceivedDate = request.ReceivedDate,
                DeliveryNoteNumber = request.DeliveryNoteNumber,
                Notes = request.Notes,
                Status = GoodsReceivedStatus.Draft,
                CreatedAt = createdAt
            };

            // Create receipt items.
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

            // Save receipt and receipt items in a transaction.
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

            // Calculate accepted quantities.
            var acceptedQuantities = receipt.Items
                .GroupBy(item => item.PurchaseOrderItemId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(item => item.AcceptedQuantity));

            // Validate accepted quantities.
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

                // Update Purchase Order received quantities.
                foreach (var accepted in acceptedQuantities)
                {
                    var poItem = poItemsDictionary[accepted.Key];
                    poItem.ReceivedQuantity += accepted.Value;
                }

                // Update Purchase Order status.
                await UpdatePurchaseOrderStatusAsync(purchaseOrder);

                // Confirm receipt.
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
                // Add released quantity to available stock.
                await AddAcceptedStockAsync(
                    receiptItem,
                    purchaseOrder,
                    poItem,
                    quantity,
                    $"Quarantine release - {receiptItem.GoodsReceived.ReceiptNumber}");

                // Move quantity from quarantine to accepted.
                receiptItem.QuarantinedQuantity -= quantity;
                receiptItem.AcceptedQuantity += quantity;

                // Update accepted Purchase Order quantity.
                poItem.ReceivedQuantity += quantity;

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

        // 8. GENERATE DAILY-SEQUENCE RECEIPT NUMBER
        private async Task<string> GenerateReceiptNumberAsync(DateTime createdAt)
        {
            var dateStart = createdAt.Date;
            var nextDate = dateStart.AddDays(1);

            // Get existing receipts created on the same UTC date.
            var existingReceipts = await goodsReceivedRepository.FindAllAsync(
                receipt =>
                    receipt.CreatedAt >= dateStart &&
                    receipt.CreatedAt < nextDate);

            var datePart = createdAt.ToString(
                "yyyyMMdd",
                CultureInfo.InvariantCulture);

            var prefix = $"GRN-{datePart}-";

            // Find the highest sequence already used on this date.
            var highestSequence = existingReceipts
                .Where(receipt =>
                    receipt.ReceiptNumber.StartsWith(prefix))
                .Select(receipt =>
                {
                    var sequencePart =
                        receipt.ReceiptNumber.Substring(prefix.Length);

                    return int.TryParse(
                        sequencePart,
                        out var sequence)
                        ? sequence
                        : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            var nextSequence = highestSequence + 1;

            return $"{prefix}{nextSequence:D3}";
        }

        // 9. LOAD RECEIPT WITH RELATED ENTITIES
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

        // 10. ADD ACCEPTED QUANTITY TO STOCK
        private async Task AddAcceptedStockAsync(
            GoodsReceivedItemEntity receiptItem,
            PurchaseOrderEntity purchaseOrder,
            PurchaseOrderItemEntity poItem,
            int quantity,
            string reason)
        {
            if (quantity <= 0)
                return;

            // Find an existing stock batch.
            var stockBatch = await stockBatchRepository.FindAsync(
                batch =>
                    batch.ItemId == poItem.ItemId &&
                    batch.BatchNumber == receiptItem.BatchNumber);

            if (stockBatch == null)
            {
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

            // Link receipt item to stock batch.
            receiptItem.StockBatch = stockBatch;

            var previousQuantity =
                stockBatch.QuantityInStock - quantity;

            // Record stock-in transaction.
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

        // 11. UPDATE PURCHASE ORDER STATUS
        private async Task UpdatePurchaseOrderStatusAsync(
            PurchaseOrderEntity purchaseOrder)
        {
            var items = await purchaseOrderItemRepository.FindAllAsync(
                item => item.PurchaseOrderId == purchaseOrder.PurchaseOrderId);

            if (!items.Any())
                return;

            if (items.All(item =>
                    item.ReceivedQuantity >= item.OrderedQuantity))
            {
                purchaseOrder.Status = PurchaseOrderStatus.Received;
            }
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
