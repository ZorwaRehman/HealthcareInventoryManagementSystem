
using FluentValidation;
using HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived;

namespace HealthcareInventoryManagementSystem.Application.Validators
{
    public class GoodsReceivedItemRequestValidator
        : AbstractValidator<CreateGoodsReceivedItemRequestDto>
    {
        public GoodsReceivedItemRequestValidator()
        {
            RuleFor(x => x.PurchaseOrderItemId)
                .GreaterThan(0)
                .WithMessage("A valid Purchase Order item is required.");

            RuleFor(x => x.BatchNumber)
                .NotEmpty()
                .WithMessage("Batch number is required.")
                .MaximumLength(50)
                .WithMessage("Batch number cannot exceed 50 characters.");

            RuleFor(x => x.DeliveredQuantity)
                .GreaterThan(0)
                .WithMessage("Delivered quantity must be greater than zero.");

            RuleFor(x => x.AcceptedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Accepted quantity cannot be negative.");

            RuleFor(x => x.RejectedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Rejected quantity cannot be negative.");

            RuleFor(x => x.QuarantinedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quarantined quantity cannot be negative.");

            RuleFor(x => x)
                .Must(x =>
                    x.DeliveredQuantity ==
                    x.AcceptedQuantity +
                    x.RejectedQuantity +
                    x.QuarantinedQuantity)
                .WithMessage(
                    "Delivered quantity must equal accepted, rejected, and quarantined quantities.");

            RuleFor(x => x)
                .Must(x =>
                    !x.ManufactureDate.HasValue ||
                    !x.ExpiryDate.HasValue ||
                    x.ExpiryDate.Value.Date >= x.ManufactureDate.Value.Date)
                .WithMessage("Expiry date cannot be earlier than manufacture date.");

            RuleFor(x => x.StorageLocation)
                .MaximumLength(100)
                .WithMessage("Storage location cannot exceed 100 characters.");
        }
    }
}
