using AutoMapper;
using HealthcareInventoryManagementSystem.Application.DTOs.Stocks;
using HealthcareInventoryManagementSystem.Application.Interfaces;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HealthcareInventoryManagementSystem.Application.Services
{
    // Implements the business logic for Stock Management.
    public class StockService(
        IGenericRepository<StockBatchEntity> stockBatchRepository,
        IGenericRepository<StockTransactionEntity> stockTransactionRepository,
        IGenericRepository<ItemEntity> itemRepository,
        IStockTransactionManager transactionManager,
        IMapper mapper,
        ILogger<StockService> logger) : IStockService
    {
        // Returns a stock summary for every inventory item.
        public async Task<IEnumerable<StockDto>> GetAllStockAsync()
        {
            try
            {
                var items = await itemRepository.GetAllAsync();
                var batches = await stockBatchRepository.GetAllAsync();

                var stockSummary = items.Select(item =>
                {
                    var totalQuantity = batches
                        .Where(batch => batch.ItemId == item.Id)
                        .Sum(batch => batch.QuantityInStock);

                    return new StockDto
                    {
                        ItemId = item.Id,
                        ItemName = item.Name,
                        TotalQuantity = totalQuantity,
                        ReorderLevel = item.ReorderLevel
                    };
                }).ToList();

                logger.LogInformation(
                    "Retrieved stock summaries for {ItemCount} items.",
                    stockSummary.Count);

                return stockSummary;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to retrieve stock summaries.");
                throw;
            }
        }

        // Returns the stock summary for one inventory item.
        public async Task<StockDto?> GetStockByItemIdAsync(int itemId)
        {
            try
            {
                var item = await itemRepository.GetByIdAsync(itemId);

                if (item is null)
                {
                    logger.LogWarning(
                        "Stock summary requested for missing item {ItemId}.",
                        itemId);

                    return null;
                }

                var batches = await stockBatchRepository.FindAllAsync(
                    batch => batch.ItemId == itemId);

                var stockDto = new StockDto
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    TotalQuantity = batches.Sum(
                        batch => batch.QuantityInStock),
                    ReorderLevel = item.ReorderLevel
                };

                logger.LogInformation(
                    "Retrieved stock summary for item {ItemId}.",
                    itemId);

                return stockDto;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to retrieve stock for item {ItemId}.",
                    itemId);

                throw;
            }
        }

        // Creates a stock batch and records its initial stock transaction.
        public async Task<StockBatchDto> CreateStockBatchAsync(
            CreateStockBatchDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.ItemId <= 0)
                throw new ArgumentException(
                    "ItemId must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.BatchNumber))
                throw new ArgumentException(
                    "Batch number is required.");

            var batchNumber = dto.BatchNumber.Trim();

            if (batchNumber.Length > 100)
                throw new ArgumentException(
                    "Batch number cannot exceed 100 characters.");

            if (dto.QuantityInStock < 0)
                throw new ArgumentException(
                    "Quantity cannot be negative.");

            if (dto.UnitPrice <= 0)
                throw new ArgumentException(
                    "Unit price must be greater than zero.");

            if (dto.ExpiryDate.HasValue &&
                dto.ExpiryDate.Value.Date < DateTime.UtcNow.Date)
            {
                throw new ArgumentException(
                    "Expiry date cannot be in the past.");
            }

            var item = await itemRepository.GetByIdAsync(dto.ItemId);

            if (item is null)
                throw new KeyNotFoundException(
                    $"Item with ID {dto.ItemId} was not found.");

            var duplicate = await stockBatchRepository.ExistsAsync(
                batch => batch.ItemId == dto.ItemId &&
                         batch.BatchNumber == batchNumber);

            if (duplicate)
                throw new ArgumentException(
                    "This batch number already exists for the selected item.");

            var batch = new StockBatchEntity
            {
                ItemId = dto.ItemId,
                BatchNumber = batchNumber,
                ExpiryDate = dto.ExpiryDate,
                QuantityInStock = dto.QuantityInStock,
                UnitPrice = dto.UnitPrice
            };

            await transactionManager.ExecuteInTransactionAsync(async () =>
            {
                // Add the batch to EF Core's change tracker.
                await stockBatchRepository.AddAsync(batch);

                // Save inside the transaction to obtain the generated batch ID.
                // The transaction manager will commit only after the operation
                // completes successfully.
                await stockBatchRepository.SaveChangesAsync();

                // Record the initial stock only when the batch has a quantity.
                if (batch.QuantityInStock > 0)
                {
                    var stockTransaction = new StockTransactionEntity
                    {
                        StockBatchId = batch.StockBatchId,
                        TransactionType = "StockIn",
                        Quantity = batch.QuantityInStock,
                        PreviousQuantity = 0,
                        NewQuantity = batch.QuantityInStock,
                        Reason = "Initial stock batch creation",
                        TransactionDate = DateTime.UtcNow
                    };

                    await stockTransactionRepository.AddAsync(
                        stockTransaction);
                }
            });

            logger.LogInformation(
                "Created stock batch {BatchNumber} for item {ItemId}.",
                batch.BatchNumber,
                batch.ItemId);

            return mapper.Map<StockBatchDto>(batch);
        }

        // Adds stock, removes stock, or sets an adjusted stock quantity.
        public async Task AdjustStockAsync(StockAdjustmentDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            try
            {
                if (dto.StockBatchId <= 0)
                    throw new ArgumentException(
                        "StockBatchId must be greater than zero.");

                if (dto.Quantity < 0)
                    throw new ArgumentException(
                        "Quantity cannot be negative.");

                var transactionType = dto.TransactionType?.Trim();

                if (transactionType is not
                    ("StockIn" or "StockOut" or "Adjustment"))
                {
                    throw new ArgumentException(
                        "TransactionType must be StockIn, StockOut, or Adjustment.");
                }

                if (transactionType != "Adjustment" && dto.Quantity == 0)
                    throw new ArgumentException(
                        "Quantity must be greater than zero for StockIn or StockOut.");

                if (dto.Reason?.Length > 500)
                    throw new ArgumentException(
                        "Reason cannot exceed 500 characters.");

                await transactionManager.ExecuteInTransactionAsync(async () =>
                {
                    var batch = await stockBatchRepository.GetByIdAsync(
                        dto.StockBatchId);

                    if (batch is null)
                        throw new KeyNotFoundException(
                            $"Stock batch {dto.StockBatchId} was not found.");

                    var previousQuantity = batch.QuantityInStock;
                    int newQuantity;

                    switch (transactionType)
                    {
                        case "StockIn":
                            newQuantity = checked(
                                previousQuantity + dto.Quantity);
                            break;

                        case "StockOut":
                            if (dto.Quantity > previousQuantity)
                                throw new InvalidOperationException(
                                    "Insufficient stock. Stock cannot become negative.");

                            newQuantity = previousQuantity - dto.Quantity;
                            break;

                        case "Adjustment":
                            newQuantity = checked(previousQuantity + dto.Quantity);
                            break;

                        default:
                            throw new ArgumentException("Invalid transaction type.");
                    }

                    batch.QuantityInStock = newQuantity;
                    stockBatchRepository.Update(batch);

                    var stockTransaction = new StockTransactionEntity
                    {
                        StockBatchId = batch.StockBatchId,
                        TransactionType = transactionType,
                        Quantity = dto.Quantity,
                        PreviousQuantity = previousQuantity,
                        NewQuantity = newQuantity,
                        Reason = dto.Reason,
                        TransactionDate = DateTime.UtcNow
                    };

                    await stockTransactionRepository.AddAsync(
                        stockTransaction);

                    // The transaction manager saves and commits both changes.
                });

                logger.LogInformation(
                    "Stock {TransactionType} completed for batch {StockBatchId}.",
                    transactionType,
                    dto.StockBatchId);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                logger.LogWarning(
                    "Stock adjustment requested for missing batch {StockBatchId}.",
                    dto.StockBatchId);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(
                    "Stock adjustment rejected for batch {StockBatchId}: {Reason}",
                    dto.StockBatchId,
                    ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Stock adjustment failed for batch {StockBatchId}.",
                    dto.StockBatchId);
                throw;
            }
        }

        // Returns transaction history for a specific stock batch.
        public async Task<IEnumerable<StockTransactionDto>>
            GetTransactionsByBatchIdAsync(int stockBatchId)
        {
            try
            {
                if (stockBatchId <= 0)
                    throw new ArgumentException(
                        "StockBatchId must be greater than zero.");

                var batch = await stockBatchRepository.GetByIdAsync(
                    stockBatchId);

                if (batch is null)
                {
                    logger.LogWarning(
                        "Transaction history requested for missing batch {StockBatchId}.",
                        stockBatchId);

                    throw new KeyNotFoundException(
                        $"Stock batch {stockBatchId} was not found.");
                }

                var transactions =
                    await stockTransactionRepository.FindAllAsync(
                        transaction =>
                            transaction.StockBatchId == stockBatchId);

                var result = mapper.Map<List<StockTransactionDto>>(
                    transactions.OrderByDescending(
                        transaction => transaction.TransactionDate));

                logger.LogInformation(
                    "Retrieved {TransactionCount} transactions for batch {StockBatchId}.",
                    result.Count,
                    stockBatchId);

                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to retrieve transaction history for batch {StockBatchId}.",
                    stockBatchId);
                throw;
            }
        }

        // Returns all stock batches for a specific inventory item.
        public async Task<IReadOnlyList<StockBatchDto>>
            GetBatchesByItemIdAsync(int itemId)
        {
            try
            {
                if (itemId <= 0)
                    throw new ArgumentException(
                        "ItemId must be greater than zero.");

                var item = await itemRepository.GetByIdAsync(itemId);

                if (item is null)
                {
                    logger.LogWarning(
                        "Stock batches requested for missing item {ItemId}.",
                        itemId);

                    throw new KeyNotFoundException(
                        $"Item {itemId} was not found.");
                }

                var batches = await stockBatchRepository.FindAllAsync(
                    batch => batch.ItemId == itemId);

                var result = mapper.Map<List<StockBatchDto>>(batches);

                logger.LogInformation(
                    "Retrieved {BatchCount} stock batches for item {ItemId}.",
                    result.Count,
                    itemId);

                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to retrieve stock batches for item {ItemId}.",
                    itemId);
                throw;
            }
        }
    }
}
