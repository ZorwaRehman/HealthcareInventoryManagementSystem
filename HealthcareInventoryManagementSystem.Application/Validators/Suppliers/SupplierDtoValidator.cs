using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using HealthcareInventoryManagementSystem.Application.DTOs.Suppliers;

namespace HealthcareInventoryManagementSystem.Application.Validators.Suppliers
{
    public class SupplierDtoValidator : AbstractValidator<SupplierRequestDto>
    {
        public SupplierDtoValidator()
        {
            // Supplier name is required and can contain up to 150 characters.
            RuleFor(supplier => supplier.Name)
                .NotEmpty()
                .WithMessage("Supplier name is required.")
                .MaximumLength(150)
                .WithMessage("Supplier name cannot exceed 150 characters.");

            // Phone number is required and can contain up to 20 characters.
            RuleFor(supplier => supplier.Phone)
                .NotEmpty()
                .WithMessage("Supplier phone number is required.")
                .MaximumLength(20)
                .WithMessage("Phone number cannot exceed 20 characters.");

            // Email is required and must have a valid email format.
            RuleFor(supplier => supplier.Email)
                .NotEmpty()
                .WithMessage("Supplier email is required.")
                .EmailAddress()
                .WithMessage("Please enter a valid email address.")
                .MaximumLength(150)
                .WithMessage("Email cannot exceed 150 characters.");

            // Supplier address is required and can contain up to 300 characters.
            RuleFor(supplier => supplier.Address)
                .NotEmpty()
                .WithMessage("Supplier address is required.")
                .MaximumLength(300)
                .WithMessage("Address cannot exceed 300 characters.");

            // Contact person is optional.
            RuleFor(supplier => supplier.ContactPerson)
                .MaximumLength(150)
                .WithMessage("Contact person name cannot exceed 150 characters.");

            // City is optional.
            RuleFor(supplier => supplier.City)
                .MaximumLength(100)
                .WithMessage("City cannot exceed 100 characters.");

            // Country is optional.
            RuleFor(supplier => supplier.Country)
                .MaximumLength(100)
                .WithMessage("Country cannot exceed 100 characters.");

            // Tax number is optional.
            RuleFor(supplier => supplier.TaxNumber)
                .MaximumLength(50)
                .WithMessage("Tax number cannot exceed 50 characters.");

            // Notes are optional.
            RuleFor(supplier => supplier.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
