
using HealthcareInventoryManagementSystem.Application.DTOs.Reports;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.Extensions.Logging;
using HealthcareInventoryManagementSystem.Application.Dtos.Pagination;
using System.Reflection;

namespace HealthcareInventoryManagementSystem.Application.Services
{
    public class ReportService(
        IGenericRepository<ItemEntity> itemRepository,
        IGenericRepository<StockBatchEntity> stockBatchRepository,
        IGenericRepository<StockTransactionEntity> stockTransactionRepository,
        IGenericRepository<PurchaseOrderEntity> purchaseOrderRepository,
        IGenericRepository<GoodsReceivedEntity> goodsReceivedRepository,
        IGenericRepository<Supplier> supplierRepository,
        ILogger<ReportService> logger) : IReportService
    {
        public async Task<IEnumerable<StockReportDto>> GetStockSummaryAsync()
        {
            try
            {
                // Get all active inventory items.
                var items = await itemRepository.FindAllAsync(
                    item => item.IsActive);

                // Get all stock batches.
                var batches = await stockBatchRepository.GetAllAsync();

                // Calculate stock information for each item.
                var report = items.Select(item =>
                {
                    var itemBatches = batches
                        .Where(batch => batch.ItemId == item.Id)
                        .ToList();

                    var totalQuantity = itemBatches
                        .Sum(batch => batch.QuantityInStock);

                    var stockValue = itemBatches
                        .Sum(batch => batch.QuantityInStock * batch.UnitPrice);

                    // Determine the current stock status.
                    var status = totalQuantity == 0
                        ? "Out of Stock"
                        : totalQuantity <= item.ReorderLevel
                            ? "Low Stock"
                            : "In Stock";

                    return new StockReportDto
                    {
                        ItemId = item.Id,
                        ItemName = item.Name,
                        SKU = item.SKU,
                        Category = item.Category,
                        Unit = item.Unit,
                        TotalQuantity = totalQuantity,
                        ReorderLevel = item.ReorderLevel,
                        StockValue = stockValue,
                        Status = status
                    };
                }).ToList();

                logger.LogInformation(
                    "Stock summary report generated successfully.");

                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error generating stock summary report.");

                throw;
            }
        }

        public async Task<IEnumerable<LowStockReportDto>> GetLowStockReportAsync()
        {
            try
            {
                // Get all active inventory items.
                var items = await itemRepository.FindAllAsync(
                    item => item.IsActive);

                // Get all stock batches.
                var batches = await stockBatchRepository.GetAllAsync();

                // Calculate stock information for each item.
                var report = items.Select(item =>
                {
                    var currentQuantity = batches
                        .Where(batch => batch.ItemId == item.Id)
                        .Sum(batch => batch.QuantityInStock);

                    // Include only items at or below their reorder level.
                    return new
                    {
                        Item = item,
                        CurrentQuantity = currentQuantity
                    };
                })
                .Where(x => x.CurrentQuantity <= x.Item.ReorderLevel)
                .Select(x => new LowStockReportDto
                {
                    ItemId = x.Item.Id,
                    ItemName = x.Item.Name,
                    SKU = x.Item.SKU,
                    Category = x.Item.Category,
                    Unit = x.Item.Unit,
                    CurrentQuantity = x.CurrentQuantity,
                    ReorderLevel = x.Item.ReorderLevel,

                    // Calculate the additional quantity required.
                    RequiredQuantity = x.Item.ReorderLevel - x.CurrentQuantity,

                    Status = x.CurrentQuantity == 0
                        ? "Out of Stock"
                        : "Low Stock"
                })
                .ToList();

                logger.LogInformation(
                    "Low stock report generated successfully.");

                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error generating low stock report.");

                throw;
            }
        }

        public async Task<IEnumerable<ExpiryReportDto>> GetExpiryReportAsync()
        {
            try
            {
                // Get all active inventory items.
                var items = await itemRepository.FindAllAsync(
                    item => item.IsActive);

                // Get all stock batches.
                var batches = await stockBatchRepository.GetAllAsync();

                // Use today's date without the time component.
                var today = DateTime.Today;

                // Prepare the expiry report.
                var report = batches
                    .Where(batch =>
                        batch.ExpiryDate.HasValue &&
                        batch.QuantityInStock > 0 &&
                        items.Any(item => item.Id == batch.ItemId))
                    .Select(batch =>
                    {
                        var item = items.First(
                            item => item.Id == batch.ItemId);

                        var expiryDate = batch.ExpiryDate!.Value;

                        var daysRemaining = (
                            expiryDate.Date - today).Days;

                        // Determine the expiry status.
                        var status = daysRemaining < 0
                            ? "Expired"
                            : daysRemaining <= 30
                                ? "Expiring Soon"
                                : "Safe";

                        return new ExpiryReportDto
                        {
                            StockBatchId = batch.StockBatchId,
                            ItemId = item.Id,
                            ItemName = item.Name,
                            SKU = item.SKU,
                            BatchNumber = batch.BatchNumber,
                            ExpiryDate = expiryDate,
                            CurrentQuantity = batch.QuantityInStock,
                            DaysRemaining = daysRemaining,
                            Status = status
                        };
                    })
                    .OrderBy(x => x.ExpiryDate)
                    .ToList();

                logger.LogInformation(
                    "Expiry report generated successfully.");

                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error generating expiry report.");

                throw;
            }
        }

        public async Task<IEnumerable<StockTransactionReportDto>> GetStockTransactionsReportAsync()
        {
            try
            {
                // Get all stock transactions with their related batch and item.
                var transactions = await stockTransactionRepository
                    .FindAllWithNestedIncludesAsync(
                        transaction => true,
                        "StockBatch.Item");

                // Prepare the stock transactions report.
                var report = transactions
                    .Where(transaction =>
                        transaction.StockBatch != null &&
                        transaction.StockBatch.Item != null &&
                        transaction.StockBatch.Item.IsActive)
                    .OrderByDescending(transaction => transaction.TransactionDate)
                    .Select(transaction => new StockTransactionReportDto
                    {
                        StockTransactionId = transaction.StockTransactionId,
                        StockBatchId = transaction.StockBatchId,
                        ItemId = transaction.StockBatch.Item.Id,
                        ItemName = transaction.StockBatch.Item.Name,
                        SKU = transaction.StockBatch.Item.SKU,
                        BatchNumber = transaction.StockBatch.BatchNumber,
                        TransactionType = transaction.TransactionType,
                        Quantity = transaction.Quantity,
                        PreviousQuantity = transaction.PreviousQuantity,
                        NewQuantity = transaction.NewQuantity,
                        Reason = transaction.Reason,
                        TransactionDate = transaction.TransactionDate
                    })
                    .ToList();

                logger.LogInformation(
                    "Stock transactions report generated successfully.");

                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error generating stock transactions report.");

                throw;
            }
        }

        // Generates a report containing purchase order details and quantity summaries.
        public async Task<IEnumerable<PurchaseReportDto>> GetPurchaseOrdersReportAsync()
        {
            try
            {
                // Retrieve purchase orders with their suppliers and items.
                var purchaseOrders = await purchaseOrderRepository
                    .FindAllWithNestedIncludesAsync(
                        purchaseOrder => true,
                        "Supplier",
                        "Items");

                // Convert purchase orders into report DTOs.
                var report = purchaseOrders
                    .OrderByDescending(purchaseOrder => purchaseOrder.OrderDate)
                    .Select(purchaseOrder => new PurchaseReportDto
                    {
                        PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                        OrderNumber = purchaseOrder.OrderNumber,
                        SupplierId = purchaseOrder.SupplierId,
                        SupplierName = purchaseOrder.Supplier.Name,
                        OrderDate = purchaseOrder.OrderDate,
                        ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
                        Status = purchaseOrder.Status.ToString(),

                        ItemCount = purchaseOrder.Items.Count,

                        TotalOrderedQuantity = purchaseOrder.Items
                            .Sum(item => item.OrderedQuantity),

                        TotalReceivedQuantity = purchaseOrder.Items
                            .Sum(item => item.ReceivedQuantity),

                        TotalRemainingQuantity = purchaseOrder.Items
                            .Sum(item => item.OrderedQuantity - item.ReceivedQuantity),

                        Subtotal = purchaseOrder.Subtotal,
                        TaxRate = purchaseOrder.TaxRate,
                        TaxAmount = purchaseOrder.TaxAmount,
                        TotalAmount = purchaseOrder.TotalAmount,
                        Notes = purchaseOrder.Notes
                    })
                    .ToList();

                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating purchase orders report.");
                throw;
            }
        }

        // Generates a report containing goods received documents and quantity summaries.
        public async Task<IEnumerable<GoodsReceivedReportDto>> GetGoodsReceivedReportAsync()
        {
            try
            {
                // Retrieve goods received documents with purchase orders,
                // suppliers, and receipt items.
                var goodsReceivedRecords = await goodsReceivedRepository
                    .FindAllWithNestedIncludesAsync(
                        receipt => true,
                        "PurchaseOrder.Supplier",
                        "Items");

                // Convert goods received documents into report DTOs.
                var report = goodsReceivedRecords
                    .OrderByDescending(receipt => receipt.ReceivedDate)
                    .Select(receipt => new GoodsReceivedReportDto
                    {
                        GoodsReceivedId = receipt.GoodsReceivedId,
                        ReceiptNumber = receipt.ReceiptNumber,
                        PurchaseOrderId = receipt.PurchaseOrderId,
                        OrderNumber = receipt.PurchaseOrder.OrderNumber,
                        SupplierName = receipt.PurchaseOrder.Supplier.Name,
                        ReceivedDate = receipt.ReceivedDate,
                        DeliveryNoteNumber = receipt.DeliveryNoteNumber,
                        Status = receipt.Status.ToString(),

                        ItemCount = receipt.Items.Count,

                        TotalDeliveredQuantity = receipt.Items
                            .Sum(item => item.DeliveredQuantity),

                        TotalAcceptedQuantity = receipt.Items
                            .Sum(item => item.AcceptedQuantity),

                        TotalRejectedQuantity = receipt.Items
                            .Sum(item => item.RejectedQuantity),

                        TotalQuarantinedQuantity = receipt.Items
                            .Sum(item => item.QuarantinedQuantity),

                        Notes = receipt.Notes
                    })
                    .ToList();

                logger.LogInformation(
                    "Goods Received report generated successfully.");

                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error generating Goods Received report.");

                throw;
            }
        }

        // Generates a report containing supplier details and stock summaries.
        public async Task<IEnumerable<SupplierReportDto>> GetSupplierReportAsync()
        {
            try
            {
                // Retrieve all suppliers, including inactive suppliers.
                var suppliers = await supplierRepository.GetAllAsync();

                // Retrieve all stock batches.
                var batches = await stockBatchRepository.GetAllAsync();

                // Calculate stock information for each supplier.
                var report = suppliers
                    .Select(supplier =>
                    {
                        // Get batches belonging to the current supplier.
                        var supplierBatches = batches
                            .Where(batch => batch.SupplierId == supplier.SupplierId)
                            .ToList();

                        return new SupplierReportDto
                        {
                            SupplierId = supplier.SupplierId,
                            SupplierName = supplier.Name,
                            ContactPerson = supplier.ContactPerson,
                            Phone = supplier.Phone,
                            Email = supplier.Email,
                            City = supplier.City,
                            Country = supplier.Country,

                            TotalStockBatches = supplierBatches.Count,

                            TotalQuantityInStock = supplierBatches
                                .Sum(batch => batch.QuantityInStock),

                            TotalStockValue = supplierBatches
                                .Sum(batch => batch.QuantityInStock * batch.UnitPrice),

                            IsActive = supplier.IsActive
                        };
                    })
                    .OrderBy(supplier => supplier.SupplierName)
                    .ToList();

                logger.LogInformation(
                    "Supplier report generated successfully.");

                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error generating supplier report.");

                throw;
            }
        }

        // Generates a report containing inventory batch details and current stock valuation.
        public async Task<IEnumerable<InventoryValuationReportDto>>
            GetInventoryValuationReportAsync()
        {
            try
            {
                // Retrieve all active inventory items.
                var items = await itemRepository.FindAllAsync(
                    item => item.IsActive);

                // Retrieve all stock batches.
                var batches = await stockBatchRepository.GetAllAsync();

                // Prepare the inventory valuation report.
                var report = batches
                    .Where(batch =>
                        batch.QuantityInStock > 0 &&
                        items.Any(item => item.Id == batch.ItemId))
                    .Select(batch =>
                    {
                        // Find the inventory item associated with this batch.
                        var item = items.First(
                            item => item.Id == batch.ItemId);

                        return new InventoryValuationReportDto
                        {
                            ItemId = item.Id,
                            ItemName = item.Name,
                            SKU = item.SKU,
                            Category = item.Category,
                            Unit = item.Unit,

                            StockBatchId = batch.StockBatchId,
                            BatchNumber = batch.BatchNumber,
                            ExpiryDate = batch.ExpiryDate,

                            QuantityInStock = batch.QuantityInStock,
                            UnitPrice = batch.UnitPrice,

                            TotalValue =
                                batch.QuantityInStock * batch.UnitPrice
                        };
                    })
                    .OrderBy(x => x.ItemName)
                    .ThenBy(x => x.BatchNumber)
                    .ToList();

                logger.LogInformation(
                    "Inventory valuation report generated successfully.");

                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error generating inventory valuation report.");

                throw;
            }
        }

        // ==========================================
        // PAGINATED REPORT METHODS
        // ==========================================

        public async Task<PaginationResponseDto<StockReportDto>>
            GetPagedStockSummaryAsync(
                int pageNumber, int pageSize, string? search, string? status)
        {
            var report = await GetStockSummaryAsync();

            return CreatePagedResponse(report, pageNumber, pageSize, search, status);
        }

        public async Task<PaginationResponseDto<LowStockReportDto>>
            GetPagedLowStockReportAsync(
                int pageNumber, int pageSize, string? search, string? status)
        {
            var report = await GetLowStockReportAsync();

            return CreatePagedResponse(report, pageNumber, pageSize, search, status);
        }

        public async Task<PaginationResponseDto<ExpiryReportDto>>
            GetPagedExpiryReportAsync(
                int pageNumber, int pageSize, string? search, string? status)
        {
            var report = await GetExpiryReportAsync();

            return CreatePagedResponse(report, pageNumber, pageSize, search, status);
        }

        public async Task<PaginationResponseDto<StockTransactionReportDto>>
            GetPagedStockTransactionsReportAsync(
                int pageNumber, int pageSize, string? search, string? status)
        {
            var report = await GetStockTransactionsReportAsync();

            return CreatePagedResponse(report, pageNumber, pageSize, search, status);
        }

        public async Task<PaginationResponseDto<PurchaseReportDto>>
            GetPagedPurchaseOrdersReportAsync(
                int pageNumber, int pageSize, string? search, string? status)
        {
            var report = await GetPurchaseOrdersReportAsync();

            return CreatePagedResponse(report, pageNumber, pageSize, search, status);
        }

        public async Task<PaginationResponseDto<GoodsReceivedReportDto>>
            GetPagedGoodsReceivedReportAsync(
                int pageNumber, int pageSize, string? search, string? status)
        {
            var report = await GetGoodsReceivedReportAsync();

            return CreatePagedResponse(report, pageNumber, pageSize, search, status);
        }

        public async Task<PaginationResponseDto<SupplierReportDto>>
            GetPagedSupplierReportAsync(
                int pageNumber, int pageSize, string? search, string? status)
        {
            var report = await GetSupplierReportAsync();

            return CreatePagedResponse(report, pageNumber, pageSize, search, status);
        }

        public async Task<PaginationResponseDto<InventoryValuationReportDto>>
            GetPagedInventoryValuationReportAsync(
                int pageNumber, int pageSize, string? search, string? status)
        {
            var report = await GetInventoryValuationReportAsync();

            return CreatePagedResponse(report, pageNumber, pageSize, search, status);
        }

        // ==========================================
        // PAGINATION HELPER
        // ==========================================

        private static PaginationResponseDto<T> CreatePagedResponse<T>(
            IEnumerable<T> source,
            int pageNumber,
            int pageSize,
            string? search,
            string? status)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            IEnumerable<T> result = source;

            // Apply search across the report DTO properties.
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();

                result = result.Where(item =>
                    item != null &&
                    typeof(T).GetProperties()
                        .Any(property =>
                        {
                            var value = property.GetValue(item);

                            return value != null &&
                                   value.ToString()!
                                       .Contains(
                                           searchTerm,
                                           StringComparison.OrdinalIgnoreCase);
                        }));
            }

            // Apply status filtering when a report has a status field.
            if (!string.IsNullOrWhiteSpace(status) &&
                !status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                var properties = typeof(T).GetProperties();

                var statusProperty = properties.FirstOrDefault(
                    property => property.Name == "Status");

                var activeProperty = properties.FirstOrDefault(
                    property => property.Name == "IsActive");

                if (statusProperty != null)
                {
                    result = result.Where(item =>
                        string.Equals(
                            statusProperty.GetValue(item)?.ToString(),
                            status,
                            StringComparison.OrdinalIgnoreCase));
                }
                else if (activeProperty != null &&
                         activeProperty.PropertyType == typeof(bool))
                {
                    bool isActive = status.Equals(
                        "Active",
                        StringComparison.OrdinalIgnoreCase);

                    bool isInactive = status.Equals(
                        "Inactive",
                        StringComparison.OrdinalIgnoreCase);

                    result = isActive || isInactive
                        ? result.Where(item =>
                            (bool)activeProperty.GetValue(item)! == isActive)
                        : Enumerable.Empty<T>();
                }
                else
                {
                    result = Enumerable.Empty<T>();
                }
            }

            var filteredItems = result.ToList();

            var totalCount = filteredItems.Count;

            var pagedItems = filteredItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginationResponseDto<T>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
