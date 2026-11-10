using System;
using System.Collections.Generic;
using System.Text;
using HealthcareInventoryManagementSystem.Application.DTOs.Suppliers;

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
    }
}
