using AutoMapper;
using FluentValidation;
using HealthcareInventoryManagementSystem.Application.DTOs.Suppliers;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HealthcareInventoryManagementSystem.Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IGenericRepository<Supplier> _supplierRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<SupplierRequestDto> _validator;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(
            IGenericRepository<Supplier> supplierRepository,
            IMapper mapper,
            IValidator<SupplierRequestDto> validator,
            ILogger<SupplierService> logger)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }

        // Get all suppliers.
        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        // Get a supplier by its ID.
        public async Task<SupplierDto?> GetSupplierByIdAsync(int supplierId)
        {
            var supplier = await _supplierRepository.GetByIdAsync(supplierId);

            if (supplier == null)
            {
                _logger.LogWarning(
                    "Supplier with ID {SupplierId} was not found.",
                    supplierId);

                return null;
            }

            return _mapper.Map<SupplierDto>(supplier);
        }

        // Create a new supplier.
        public async Task<SupplierDto> CreateSupplierAsync(
            SupplierRequestDto supplierRequest)
        {
            await _validator.ValidateAndThrowAsync(supplierRequest);

            var supplier = _mapper.Map<Supplier>(supplierRequest);

            supplier.CreatedAt = DateTime.UtcNow;
            supplier.IsActive = true;

            await _supplierRepository.AddAsync(supplier);
            await _supplierRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Supplier {SupplierName} was created successfully.",
                supplier.Name);

            return _mapper.Map<SupplierDto>(supplier);
        }

        // Update an existing supplier.
        public async Task<SupplierDto?> UpdateSupplierAsync(
            int supplierId,
            SupplierRequestDto supplierRequest)
        {
            var supplier = await _supplierRepository.GetByIdAsync(supplierId);

            if (supplier == null)
            {
                _logger.LogWarning(
                    "Supplier with ID {SupplierId} was not found for update.",
                    supplierId);

                return null;
            }

            await _validator.ValidateAndThrowAsync(supplierRequest);

            _mapper.Map(supplierRequest, supplier);

            supplier.UpdatedAt = DateTime.UtcNow;

            _supplierRepository.Update(supplier);
            await _supplierRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Supplier with ID {SupplierId} was updated successfully.",
                supplierId);

            return _mapper.Map<SupplierDto>(supplier);
        }

        // Deactivate a supplier without permanently deleting its record.
        public async Task<bool> DeactivateSupplierAsync(int supplierId)
        {
            var supplier = await _supplierRepository.GetByIdAsync(supplierId);

            if (supplier == null)
            {
                _logger.LogWarning(
                    "Supplier with ID {SupplierId} was not found for deactivation.",
                    supplierId);

                return false;
            }

            if (!supplier.IsActive)
            {
                return true;
            }

            supplier.IsActive = false;
            supplier.UpdatedAt = DateTime.UtcNow;

            _supplierRepository.Update(supplier);
            await _supplierRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Supplier with ID {SupplierId} was deactivated successfully.",
                supplierId);

            return true;
        }

        // Reactivate an inactive supplier.
        public async Task<bool> ReactivateSupplierAsync(int supplierId)
        {
            var supplier = await _supplierRepository.GetByIdAsync(supplierId);

            if (supplier == null)
            {
                _logger.LogWarning(
                    "Supplier with ID {SupplierId} was not found for reactivation.",
                    supplierId);

                return false;
            }

            if (supplier.IsActive)
            {
                return true;
            }

            supplier.IsActive = true;
            supplier.UpdatedAt = DateTime.UtcNow;

            _supplierRepository.Update(supplier);
            await _supplierRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Supplier with ID {SupplierId} was reactivated successfully.",
                supplierId);

            return true;
        }
    }
}