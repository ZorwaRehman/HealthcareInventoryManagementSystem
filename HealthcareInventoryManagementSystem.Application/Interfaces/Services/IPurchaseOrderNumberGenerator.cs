
using System.Threading.Tasks;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface IPurchaseOrderNumberGenerator
    {
        Task<string> GenerateNextNumberAsync();
    }
}
