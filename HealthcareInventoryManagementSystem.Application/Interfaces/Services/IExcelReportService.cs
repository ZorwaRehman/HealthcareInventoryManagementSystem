
namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface IExcelReportService
    {
        byte[] GenerateExcel<T>(
            IEnumerable<T> data,
            string worksheetName,
            string reportTitle,
            string currencyCode = "PKR",
            decimal exchangeRate = 1m);
    }
}
