using HealthcareInventoryManagementSystem.Application.DTOs.Reports;
using HealthcareInventoryManagementSystem.Application.Dtos.Pagination;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface IReportService
    {
        // Existing report methods.
        Task<IEnumerable<StockReportDto>> GetStockSummaryAsync();

        Task<IEnumerable<LowStockReportDto>> GetLowStockReportAsync();

        Task<IEnumerable<ExpiryReportDto>> GetExpiryReportAsync();

        Task<IEnumerable<StockTransactionReportDto>> GetStockTransactionsReportAsync();

        Task<IEnumerable<PurchaseReportDto>> GetPurchaseOrdersReportAsync();

        Task<IEnumerable<GoodsReceivedReportDto>> GetGoodsReceivedReportAsync();

        Task<IEnumerable<SupplierReportDto>> GetSupplierReportAsync();

        Task<IEnumerable<InventoryValuationReportDto>> GetInventoryValuationReportAsync();

        // New paginated report methods.
        Task<PaginationResponseDto<StockReportDto>> GetPagedStockSummaryAsync(
            int pageNumber, int pageSize, string? search, string? status);

        Task<PaginationResponseDto<LowStockReportDto>> GetPagedLowStockReportAsync(
            int pageNumber, int pageSize, string? search, string? status);

        Task<PaginationResponseDto<ExpiryReportDto>> GetPagedExpiryReportAsync(
            int pageNumber, int pageSize, string? search, string? status);

        Task<PaginationResponseDto<StockTransactionReportDto>> GetPagedStockTransactionsReportAsync(
            int pageNumber, int pageSize, string? search, string? status);

        Task<PaginationResponseDto<PurchaseReportDto>> GetPagedPurchaseOrdersReportAsync(
            int pageNumber, int pageSize, string? search, string? status);

        Task<PaginationResponseDto<GoodsReceivedReportDto>> GetPagedGoodsReceivedReportAsync(
            int pageNumber, int pageSize, string? search, string? status);

        Task<PaginationResponseDto<SupplierReportDto>> GetPagedSupplierReportAsync(
            int pageNumber, int pageSize, string? search, string? status);

        Task<PaginationResponseDto<InventoryValuationReportDto>> GetPagedInventoryValuationReportAsync(
            int pageNumber, int pageSize, string? search, string? status);
    }
}