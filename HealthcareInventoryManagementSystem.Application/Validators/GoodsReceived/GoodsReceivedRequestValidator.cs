
using FluentValidation;
using HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived;

namespace HealthcareInventoryManagementSystem.Application.Validators
{
    public class GoodsReceivedRequestValidator
        : AbstractValidator<CreateGoodsReceivedRequestDto>
    {
        public GoodsReceivedRequestValidator()
        {
            RuleFor(x => x.PurchaseOrderId)
                .GreaterThan(0)
                .WithMessage("A valid Purchase Order is required.");

            RuleFor(x => x.ReceivedDate)
                .NotEmpty()
                .WithMessage("Received date is required.");

            RuleFor(x => x.DeliveryNoteNumber)
                .MaximumLength(100)
                .WithMessage("Delivery note number cannot exceed 100 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters.");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one received item is required.");

            RuleForEach(x => x.Items)
                .SetValidator(new GoodsReceivedItemRequestValidator());
        }
    }
}
