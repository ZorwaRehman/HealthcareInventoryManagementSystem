
using FluentValidation;
using HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders;

namespace HealthcareInventoryManagementSystem.Application.Validators.PurchaseOrders
{
    public class PurchaseOrderItemRequestDtoValidator
        : AbstractValidator<PurchaseOrderItemRequestDto>
    {
        public PurchaseOrderItemRequestDtoValidator()
        {
            // An existing inventory item must be selected.
            RuleFor(item => item.ItemId)
                .GreaterThan(0)
                .WithMessage("Please select a valid inventory item.");

            // Ordered quantity must be greater than zero.
            RuleFor(item => item.OrderedQuantity)
                .GreaterThan(0)
                .WithMessage("Ordered quantity must be greater than zero.");

            // Unit price cannot be negative.
            RuleFor(item => item.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Unit price cannot be negative.");
        }
    }
}
