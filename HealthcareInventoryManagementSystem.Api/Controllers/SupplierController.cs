
using HealthcareInventoryManagementSystem.Application.DTOs.Suppliers;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareInventoryManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();

            return Ok(suppliers);
        }

        // GET: api/Suppliers/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
            {
                return NotFound(new
                {
                    message = $"Supplier with ID {id} was not found."
                });
            }

            return Ok(supplier);
        }

        // POST: api/Suppliers
        [HttpPost]
        public async Task<IActionResult> CreateSupplier(
            [FromBody] SupplierRequestDto request)
        {
            try
            {
                var supplier = await _supplierService.CreateSupplierAsync(request);

                return CreatedAtAction(
                    nameof(GetSupplierById),
                    new { id = supplier.SupplierId },
                    supplier);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/Suppliers/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSupplier(
            int id,
            [FromBody] SupplierRequestDto request)
        {
            try
            {
                var supplier = await _supplierService.UpdateSupplierAsync(id, request);

                if (supplier == null)
                {
                    return NotFound(new
                    {
                        message = $"Supplier with ID {id} was not found."
                    });
                }

                return Ok(supplier);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // DELETE: api/Suppliers/5
        // Soft delete: deactivates the supplier.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeactivateSupplier(int id)
        {
            var deactivated = await _supplierService.DeactivateSupplierAsync(id);

            if (!deactivated)
            {
                return NotFound(new
                {
                    message = $"Supplier with ID {id} was not found."
                });
            }

            return NoContent();
        }

        // PATCH: api/Suppliers/5/reactivate
        [HttpPatch("{id:int}/reactivate")]
        public async Task<IActionResult> ReactivateSupplier(int id)
        {
            var reactivated = await _supplierService.ReactivateSupplierAsync(id);

            if (!reactivated)
            {
                return NotFound(new
                {
                    message = $"Supplier with ID {id} was not found."
                });
            }

            return Ok(new
            {
                message = "Supplier reactivated successfully."
            });
        }
    }
}
