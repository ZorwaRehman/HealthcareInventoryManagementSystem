
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HealthcareInventoryManagementSystem.Application.DTOs.Alerts;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Entities;

namespace HealthcareInventoryManagementSystem.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly IGenericRepository<ItemEntity> _itemRepository;
        private readonly IGenericRepository<StockBatchEntity> _stockBatchRepository;

        public AlertService(
            IGenericRepository<ItemEntity> itemRepository,
            IGenericRepository<StockBatchEntity> stockBatchRepository)
        {
            _itemRepository = itemRepository;
            _stockBatchRepository = stockBatchRepository;
        }

        // Get low-stock and expiry alerts together.
        public async Task<IEnumerable<AlertDto>> GetAllAlertsAsync()
        {
            var lowStockAlerts = await GetLowStockAlertsAsync();
            var expiryAlerts = await GetExpiryAlertsAsync();

            return lowStockAlerts
                .Concat(expiryAlerts)
                .OrderByDescending(alert => alert.CreatedAt)
                .ToList();
        }

        // Find items whose total stock is at or below their reorder level.
        public async Task<IEnumerable<AlertDto>> GetLowStockAlertsAsync()
        {
            var items = await _itemRepository.GetAllAsync();
            var batches = await _stockBatchRepository.GetAllAsync();

            var alerts = new List<AlertDto>();

            foreach (var item in items)
            {
                if (!item.IsActive)
                    continue;

                var totalQuantity = batches
                    .Where(batch => batch.ItemId == item.Id)
                    .Sum(batch => batch.QuantityInStock);

                if (totalQuantity <= item.ReorderLevel)
                {
                    alerts.Add(new AlertDto
                    {
                        AlertId = 0, // Generated alert; not saved in the database.
                        AlertType = "LowStock",
                        Message = $"Low stock for {item.Name}. Current quantity: {totalQuantity}. Reorder level: {item.ReorderLevel}.",
                        Severity = totalQuantity == 0 ? "Critical" : "Warning",
                        ItemId = item.Id,
                        ItemName = item.Name,
                        CurrentQuantity = totalQuantity,
                        ReorderLevel = item.ReorderLevel,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    });
                }
            }

            return alerts;
        }

        // Find batches that are expired or approaching expiry.
        public async Task<IEnumerable<AlertDto>> GetExpiryAlertsAsync()
        {
            var batches = await _stockBatchRepository.GetAllAsync();

            var today = DateTime.UtcNow.Date;
            var expiryWarningDate = today.AddDays(30);

            var alerts = new List<AlertDto>();

            foreach (var batch in batches)
            {
                // Ignore batches without an expiry date or without stock.
                if (!batch.ExpiryDate.HasValue ||
                    batch.QuantityInStock <= 0)
                {
                    continue;
                }

                var expiryDate = batch.ExpiryDate.Value.Date;

                if (expiryDate <= expiryWarningDate)
                {
                    var isExpired = expiryDate < today;

                    alerts.Add(new AlertDto
                    {
                        AlertId = 0, // Generated alert; not saved in the database.
                        AlertType = isExpired ? "ExpiredStock" : "ExpiringStock",
                        Message = isExpired
                            ? $"Batch {batch.BatchNumber} has expired."
                            : $"Batch {batch.BatchNumber} is expiring on {expiryDate:yyyy-MM-dd}.",
                        Severity = isExpired ? "Critical" : "Warning",
                        ItemId = batch.ItemId,
                        StockBatchId = batch.StockBatchId,
                        BatchNumber = batch.BatchNumber,
                        CurrentQuantity = batch.QuantityInStock,
                        ExpiryDate = expiryDate,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    });
                }
            }

            return alerts;
        }
    }
}
