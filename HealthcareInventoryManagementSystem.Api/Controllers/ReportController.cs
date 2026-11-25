
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using HealthcareInventoryManagementSystem.Application.Dtos.Pagination;

namespace HealthcareInventoryManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController(
        IReportService reportService,
        IExcelReportService excelReportService,
        IPdfReportService pdfReportService) : ControllerBase
    {
        // GET: api/Report/stock-summary
        [HttpGet("stock-summary")]
        public async Task<IActionResult> GetStockSummary()
        {
            var report = await reportService.GetStockSummaryAsync();

            return Ok(report);
        }

        // GET: api/Report/stock-summary/excel
        [HttpGet("stock-summary/excel")]
        public async Task<IActionResult> ExportStockSummaryExcel()
        {
            var report = await reportService.GetStockSummaryAsync();

            var file = excelReportService.GenerateExcel(
                report,
                "Stock Summary",
                "Stock Summary Report");

            var fileName = $"Stock-Summary-{DateTime.Now:yyyyMMdd}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // GET: api/Report/stock-summary/pdf
        [HttpGet("stock-summary/pdf")]
        public async Task<IActionResult> ExportStockSummaryPdf()
        {
            var report = await reportService.GetStockSummaryAsync();

            var file = pdfReportService.GeneratePdf(
                report,
                "Stock Summary Report");

            var fileName = $"Stock-Summary-{DateTime.Now:yyyyMMdd}.pdf";

            return File(
                file,
                "application/pdf",
                fileName);
        }

        // GET: api/Report/low-stock
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockReport()
        {
            var report = await reportService.GetLowStockReportAsync();

            return Ok(report);
        }

        // GET: api/Report/low-stock/excel
        [HttpGet("low-stock/excel")]
        public async Task<IActionResult> ExportLowStockExcel()
        {
            var report = await reportService.GetLowStockReportAsync();

            var file = excelReportService.GenerateExcel(
                report,
                "Low Stock",
                "Low Stock Report");

            var fileName = $"Low-Stock-{DateTime.Now:yyyyMMdd}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // GET: api/Report/low-stock/pdf
        [HttpGet("low-stock/pdf")]
        public async Task<IActionResult> ExportLowStockPdf()
        {
            var report = await reportService.GetLowStockReportAsync();

            var file = pdfReportService.GeneratePdf(
                report,
                "Low Stock Report");

            var fileName = $"Low-Stock-{DateTime.Now:yyyyMMdd}.pdf";

            return File(
                file,
                "application/pdf",
                fileName);
        }

        // GET: api/Report/expiry
        [HttpGet("expiry")]
        public async Task<IActionResult> GetExpiryReport()
        {
            var report = await reportService.GetExpiryReportAsync();

            return Ok(report);
        }

        // GET: api/Report/expiry/excel
        [HttpGet("expiry/excel")]
        public async Task<IActionResult> ExportExpiryExcel()
        {
            var report = await reportService.GetExpiryReportAsync();

            var file = excelReportService.GenerateExcel(
                report,
                "Expiry Report",
                "Expiry Report");

            var fileName = $"Expiry-Report-{DateTime.Now:yyyyMMdd}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // GET: api/Report/expiry/pdf
        [HttpGet("expiry/pdf")]
        public async Task<IActionResult> ExportExpiryPdf()
        {
            var report = await reportService.GetExpiryReportAsync();

            var file = pdfReportService.GeneratePdf(
                report,
                "Expiry Report");

            var fileName = $"Expiry-Report-{DateTime.Now:yyyyMMdd}.pdf";

            return File(
                file,
                "application/pdf",
                fileName);
        }

        // GET: api/Report/stock-transactions
        [HttpGet("stock-transactions")]
        public async Task<IActionResult> GetStockTransactionsReport()
        {
            var report = await reportService.GetStockTransactionsReportAsync();

            return Ok(report);
        }

        [HttpGet("stock-transactions/excel")]
        public async Task<IActionResult> ExportStockTransactionsExcel()
        {
            var report = await reportService.GetStockTransactionsReportAsync();

            // Exclude Reason from the downloaded Excel file.
            var exportData = report.Select(transaction => new
            {
                transaction.StockTransactionId,
                transaction.StockBatchId,
                transaction.ItemId,
                transaction.ItemName,
                transaction.SKU,
                transaction.BatchNumber,
                transaction.TransactionType,
                transaction.Quantity,
                transaction.PreviousQuantity,
                transaction.NewQuantity,
                transaction.TransactionDate
            });

            var file = excelReportService.GenerateExcel(
                exportData,
                "Stock Transactions",
                "Stock Transactions Report");

            var fileName = $"Stock-Transactions-{DateTime.Now:yyyyMMdd}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        [HttpGet("stock-transactions/pdf")]
        public async Task<IActionResult> ExportStockTransactionsPdf()
        {
            var report = await reportService.GetStockTransactionsReportAsync();

            // Exclude Reason from the downloaded PDF file.
            var exportData = report.Select(transaction => new
            {
                transaction.StockTransactionId,
                transaction.StockBatchId,
                transaction.ItemId,
                transaction.ItemName,
                transaction.SKU,
                transaction.BatchNumber,
                transaction.TransactionType,
                transaction.Quantity,
                transaction.PreviousQuantity,
                transaction.NewQuantity,
                transaction.TransactionDate
            });

            var file = pdfReportService.GeneratePdf(
                exportData,
                "Stock Transactions Report");

            var fileName = $"Stock-Transactions-{DateTime.Now:yyyyMMdd}.pdf";

            return File(
                file,
                "application/pdf",
                fileName);
        }


        // GET: api/Report/purchase-orders
        [HttpGet("purchase-orders")]
        public async Task<IActionResult> GetPurchaseOrdersReport()
        {
            var report = await reportService.GetPurchaseOrdersReportAsync();

            return Ok(report);
        }



        // GET: api/Report/purchase-orders/excel
        [HttpGet("purchase-orders/excel")]
        public async Task<IActionResult> ExportPurchaseOrdersReportToExcel(
            [FromQuery] string currencyCode = "PKR",
            [FromQuery] decimal exchangeRate = 1m)
        {
            var report = await reportService.GetPurchaseOrdersReportAsync();

            // Exclude Notes from the exported Excel report.
            var exportData = report.Select(order => new
            {
                order.PurchaseOrderId,
                order.OrderNumber,
                order.SupplierId,
                order.SupplierName,
                order.OrderDate,
                order.ExpectedDeliveryDate,
                order.Status,
                order.ItemCount,
                order.TotalOrderedQuantity,
                order.TotalReceivedQuantity,
                order.TotalRemainingQuantity,
                order.Subtotal,
                order.TaxRate,
                order.TaxAmount,
                order.TotalAmount
            });

            var file = excelReportService.GenerateExcel(
                exportData,
                "Purchase Orders",
                "Purchase Orders Report",
                currencyCode,
                exchangeRate);

            var fileName = $"Purchase-Orders-Report-{DateTime.Now:yyyyMMdd}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // GET: api/Report/purchase-orders/pdf
        [HttpGet("purchase-orders/pdf")]
        public async Task<IActionResult> ExportPurchaseOrdersReportToPdf(
            [FromQuery] string currencyCode = "PKR",
            [FromQuery] decimal exchangeRate = 1m)
        {
            var report = await reportService.GetPurchaseOrdersReportAsync();

            // Exclude Notes from the exported PDF report.
            var exportData = report.Select(order => new
            {
                order.PurchaseOrderId,
                order.OrderNumber,
                order.SupplierId,
                order.SupplierName,
                order.OrderDate,
                order.ExpectedDeliveryDate,
                order.Status,
                order.ItemCount,
                order.TotalOrderedQuantity,
                order.TotalReceivedQuantity,
                order.TotalRemainingQuantity,
                order.Subtotal,
                order.TaxRate,
                order.TaxAmount,
                order.TotalAmount
            });

            var file = pdfReportService.GeneratePdf(
                exportData,
                "Purchase Orders Report",
                currencyCode,
                exchangeRate);

            var fileName = $"Purchase-Orders-Report-{DateTime.Now:yyyyMMdd}.pdf";

            return File(
                file,
                "application/pdf",
                fileName);
        }
        // GET: api/Report/goods-received
        [HttpGet("goods-received")]
        public async Task<IActionResult> GetGoodsReceivedReport()
        {
            var report = await reportService.GetGoodsReceivedReportAsync();

            return Ok(report);
        }

        // GET: api/Report/goods-received/excel
        [HttpGet("goods-received/excel")]
        public async Task<IActionResult> ExportGoodsReceivedExcel()
        {
            var report = await reportService.GetGoodsReceivedReportAsync();

            // Exclude Notes from the downloaded Excel file.
            var exportData = report.Select(receipt => new
            {
                receipt.GoodsReceivedId,
                receipt.ReceiptNumber,
                receipt.PurchaseOrderId,
                receipt.OrderNumber,
                receipt.SupplierName,
                receipt.ReceivedDate,
                receipt.DeliveryNoteNumber,
                receipt.Status,
                receipt.ItemCount,
                receipt.TotalDeliveredQuantity,
                receipt.TotalAcceptedQuantity,
                receipt.TotalRejectedQuantity,
                receipt.TotalQuarantinedQuantity
            });

            var file = excelReportService.GenerateExcel(
                exportData,
                "Goods Received",
                "Goods Received Report");

            var fileName = $"Goods-Received-Report-{DateTime.Now:yyyyMMdd}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // GET: api/Report/goods-received/pdf
        [HttpGet("goods-received/pdf")]
        public async Task<IActionResult> ExportGoodsReceivedPdf()
        {
            var report = await reportService.GetGoodsReceivedReportAsync();

            // Exclude Notes from the downloaded PDF file.
            var exportData = report.Select(receipt => new
            {
                receipt.GoodsReceivedId,
                receipt.ReceiptNumber,
                receipt.PurchaseOrderId,
                receipt.OrderNumber,
                receipt.SupplierName,
                receipt.ReceivedDate,
                receipt.DeliveryNoteNumber,
                receipt.Status,
                receipt.ItemCount,
                receipt.TotalDeliveredQuantity,
                receipt.TotalAcceptedQuantity,
                receipt.TotalRejectedQuantity,
                receipt.TotalQuarantinedQuantity
            });

            var file = pdfReportService.GeneratePdf(
                exportData,
                "Goods Received Report");

            var fileName = $"Goods-Received-Report-{DateTime.Now:yyyyMMdd}.pdf";

            return File(
                file,
                "application/pdf",
                fileName);
        }
        // GET: api/Report/suppliers
        [HttpGet("suppliers")]
        public async Task<IActionResult> GetSupplierReport()
        {
            var report = await reportService.GetSupplierReportAsync();

            return Ok(report);
        }

        // GET: api/Report/suppliers/excel
        [HttpGet("suppliers/excel")]
        public async Task<IActionResult> ExportSupplierReportExcel(
            [FromQuery] string currencyCode = "PKR",
            [FromQuery] decimal exchangeRate = 1m)
        {
            var report = await reportService.GetSupplierReportAsync();

            var file = excelReportService.GenerateExcel(
                report,
                "Suppliers",
                "Supplier Report",
                currencyCode,
                exchangeRate);

            var fileName = $"Supplier-Report-{DateTime.Now:yyyyMMdd}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // GET: api/Report/suppliers/pdf
        [HttpGet("suppliers/pdf")]
        public async Task<IActionResult> ExportSupplierReportPdf(
            [FromQuery] string currencyCode = "PKR",
            [FromQuery] decimal exchangeRate = 1m)
        {
            var report = await reportService.GetSupplierReportAsync();

            var file = pdfReportService.GeneratePdf(
                report,
                "Supplier Report",
                currencyCode,
                exchangeRate);

            var fileName = $"Supplier-Report-{DateTime.Now:yyyyMMdd}.pdf";

            return File(
                file,
                "application/pdf",
                fileName);
        }

        // GET: api/Report/inventory-valuation
        [HttpGet("inventory-valuation")]
        public async Task<IActionResult> GetInventoryValuationReport()
        {
            var report = await reportService.GetInventoryValuationReportAsync();

            return Ok(report);
        }

        // GET: api/Report/inventory-valuation/excel
        [HttpGet("inventory-valuation/excel")]
        public async Task<IActionResult> ExportInventoryValuationExcel(
            [FromQuery] string currencyCode = "PKR",
            [FromQuery] decimal exchangeRate = 1m)
        {
            var report = await reportService.GetInventoryValuationReportAsync();

            var file = excelReportService.GenerateExcel(
                report,
                "Inventory Valuation",
                "Inventory Valuation Report",
                currencyCode,
                exchangeRate);

            var fileName = $"Inventory-Valuation-Report-{DateTime.Now:yyyyMMdd}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // GET: api/Report/inventory-valuation/pdf
        [HttpGet("inventory-valuation/pdf")]
        public async Task<IActionResult> ExportInventoryValuationPdf(
            [FromQuery] string currencyCode = "PKR",
            [FromQuery] decimal exchangeRate = 1m)
        {
            var report = await reportService.GetInventoryValuationReportAsync();

            var file = pdfReportService.GeneratePdf(
                report,
                "Inventory Valuation Report",
                currencyCode,
                exchangeRate);

            var fileName = $"Inventory-Valuation-Report-{DateTime.Now:yyyyMMdd}.pdf";

            return File(
                file,
                "application/pdf",
                fileName);
        }

        // ==========================================
        // PAGINATED REPORT ENDPOINTS
        // ==========================================

        // GET: api/Report/stock-summary/paged
        [HttpGet("stock-summary/paged")]
        public async Task<IActionResult> GetPagedStockSummary(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var report = await reportService.GetPagedStockSummaryAsync(
                pageNumber, pageSize, search, status);

            return Ok(report);
        }

        // GET: api/Report/low-stock/paged
        [HttpGet("low-stock/paged")]
        public async Task<IActionResult> GetPagedLowStockReport(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var report = await reportService.GetPagedLowStockReportAsync(
                pageNumber, pageSize, search, status);

            return Ok(report);
        }

        // GET: api/Report/expiry/paged
        [HttpGet("expiry/paged")]
        public async Task<IActionResult> GetPagedExpiryReport(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var report = await reportService.GetPagedExpiryReportAsync(
                pageNumber, pageSize, search, status);

            return Ok(report);
        }

        // GET: api/Report/stock-transactions/paged
        [HttpGet("stock-transactions/paged")]
        public async Task<IActionResult> GetPagedStockTransactionsReport(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var report = await reportService.GetPagedStockTransactionsReportAsync(
                pageNumber, pageSize, search, status);

            return Ok(report);
        }

        // GET: api/Report/purchase-orders/paged
        [HttpGet("purchase-orders/paged")]
        public async Task<IActionResult> GetPagedPurchaseOrdersReport(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var report = await reportService.GetPagedPurchaseOrdersReportAsync(
                pageNumber, pageSize, search, status);

            return Ok(report);
        }

        // GET: api/Report/goods-received/paged
        [HttpGet("goods-received/paged")]
        public async Task<IActionResult> GetPagedGoodsReceivedReport(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var report = await reportService.GetPagedGoodsReceivedReportAsync(
                pageNumber, pageSize, search, status);

            return Ok(report);
        }

        // GET: api/Report/suppliers/paged
        [HttpGet("suppliers/paged")]
        public async Task<IActionResult> GetPagedSupplierReport(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var report = await reportService.GetPagedSupplierReportAsync(
                pageNumber, pageSize, search, status);

            return Ok(report);
        }

        // GET: api/Report/inventory-valuation/paged
        [HttpGet("inventory-valuation/paged")]
        public async Task<IActionResult> GetPagedInventoryValuationReport(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var report = await reportService.GetPagedInventoryValuationReportAsync(
                pageNumber, pageSize, search, status);

            return Ok(report);
        }
    }
}
