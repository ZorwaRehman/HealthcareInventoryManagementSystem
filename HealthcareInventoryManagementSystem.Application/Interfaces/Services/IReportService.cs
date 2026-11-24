
using HealthcareInventoryManagementSystem.Application.DTOs.Reports;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<IEnumerable<StockReportDto>> GetStockSummaryAsync();

        Task<IEnumerable<LowStockReportDto>> GetLowStockReportAsync();

        Task<IEnumerable<ExpiryReportDto>> GetExpiryReportAsync();

        Task<IEnumerable<StockTransactionReportDto>> GetStockTransactionsReportAsync();

        Task<IEnumerable<PurchaseReportDto>> GetPurchaseOrdersReportAsync();

        Task<IEnumerable<GoodsReceivedReportDto>> GetGoodsReceivedReportAsync();

        Task<IEnumerable<SupplierReportDto>> GetSupplierReportAsync();

        Task<IEnumerable<InventoryValuationReportDto>> GetInventoryValuationReportAsync();
    }
}
