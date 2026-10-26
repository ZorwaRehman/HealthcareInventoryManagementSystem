
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthcareInventoryManagementSystem.Application.DTOs.Alerts;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface IAlertService
    {
        Task<IEnumerable<AlertDto>> GetAllAlertsAsync();

        Task<IEnumerable<AlertDto>> GetLowStockAlertsAsync();

        Task<IEnumerable<AlertDto>> GetExpiryAlertsAsync();
    }
}
