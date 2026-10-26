using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HealthcareInventoryManagementSystem.Application.DTOs.Stocks;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    // Defines the operations that the Stock Management service must provide.
    public interface IStockService
    {
        // Gets stock information for all inventory items.
        Task<IEnumerable<StockDto>> GetAllStockAsync();

        // Gets stock information for a specific inventory item.
        Task<StockDto?> GetStockByItemIdAsync(int itemId);

        // Adds stock, removes stock, or adjusts an existing stock quantity.
        Task AdjustStockAsync(StockAdjustmentDto dto);

        // Gets the transaction history for a specific stock batch.
        Task<IEnumerable<StockTransactionDto>> GetTransactionsByBatchIdAsync(int stockBatchId);

        // Gets all stock batches belonging to a specific item.
        Task<IReadOnlyList<StockBatchDto>> GetBatchesByItemIdAsync(int itemId);

        // Creates a new stock batch for an inventory item.
        Task<StockBatchDto> CreateStockBatchAsync(CreateStockBatchDto dto);

    }
}
