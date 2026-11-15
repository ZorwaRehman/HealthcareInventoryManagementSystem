
using System;
using FluentValidation;
using HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders;

namespace HealthcareInventoryManagementSystem.Application.Validators.PurchaseOrders
{
    public class PurchaseOrderRequestDtoValidator
        : AbstractValidator<PurchaseOrderRequestDto>
    {
        public PurchaseOrderRequestDtoValidator()
        {
            // Supplier ID must be valid.
            RuleFor(order => order.SupplierId)
                .GreaterThan(0)
                .WithMessage("Please select a valid supplier.");

            // Order date is required.
            RuleFor(order => order.OrderDate)
                .NotEmpty()
                .WithMessage("Order date is required.");

            // Expected delivery date cannot be earlier than order date.
            RuleFor(order => order.ExpectedDeliveryDate)
                .Must((order, deliveryDate) =>
                    !deliveryDate.HasValue ||
                    deliveryDate.Value.Date >= order.OrderDate.Date)
                .WithMessage("Expected delivery date cannot be earlier than order date.");

            // Tax rate must be between 0 and 100 percent.
            RuleFor(order => order.TaxRate)
                .InclusiveBetween(0, 100)
                .WithMessage("Tax rate must be between 0 and 100 percent.");

            // Notes are optional but cannot exceed 1000 characters.
            RuleFor(order => order.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters.");

            // A purchase order must contain at least one item.
            RuleFor(order => order.Items)
                .NotEmpty()
                .WithMessage("A purchase order must contain at least one item.");

            // Each item must pass its own validation rules.
            RuleForEach(order => order.Items)
                .SetValidator(new PurchaseOrderItemRequestDtoValidator());

            // The same inventory item cannot appear twice in one order.
            RuleFor(order => order.Items)
                .Must(items =>
                    items == null ||
                    items.Select(item => item.ItemId).Distinct().Count() == items.Count)
                .WithMessage("The same inventory item cannot be added more than once.");
        }
    }
}
