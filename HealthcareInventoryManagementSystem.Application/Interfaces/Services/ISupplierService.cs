
using HealthcareInventoryManagementSystem.Application.Dtos.Pagination;
using HealthcareInventoryManagementSystem.Application.DTOs.Suppliers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface ISupplierService
    {
        // Get all suppliers.
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();

        // Get a supplier by its ID.
        Task<SupplierDto?> GetSupplierByIdAsync(int supplierId);

        // Create a new supplier.
        Task<SupplierDto> CreateSupplierAsync(SupplierRequestDto supplierRequest);

        // Update an existing supplier.
        Task<SupplierDto?> UpdateSupplierAsync(
            int supplierId,
            SupplierRequestDto supplierRequest);

        // Deactivate a supplier without permanently deleting its record.
        Task<bool> DeactivateSupplierAsync(int supplierId);

        // Reactivate an inactive supplier.
        Task<bool> ReactivateSupplierAsync(int supplierId);

        // Get suppliers using server-side pagination, search, and status filtering.
        Task<PaginationResponseDto<SupplierDto>> GetPagedSuppliersAsync(
            PaginationRequestDto request,
            string? search = null,
            bool? isActive = null);
    }
}
