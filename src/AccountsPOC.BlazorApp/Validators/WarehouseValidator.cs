using FluentValidation;
using AccountsPOC.BlazorApp.Models;

namespace AccountsPOC.BlazorApp.Validators;

public class WarehouseValidator : AbstractValidator<Warehouse>
{
    public WarehouseValidator()
    {
        RuleFor(w => w.WarehouseCode)
            .NotEmpty().WithMessage("Warehouse Code is required")
            .MaximumLength(50).WithMessage("Warehouse Code must not exceed 50 characters");

        RuleFor(w => w.WarehouseName)
            .NotEmpty().WithMessage("Warehouse Name is required")
            .MaximumLength(200).WithMessage("Warehouse Name must not exceed 200 characters");

        RuleFor(w => w.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters");

        RuleFor(w => w.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters")
            .When(w => !string.IsNullOrEmpty(w.City));

        RuleFor(w => w.PostCode)
            .MaximumLength(20).WithMessage("PostCode must not exceed 20 characters")
            .When(w => !string.IsNullOrEmpty(w.PostCode));

        RuleFor(w => w.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters")
            .When(w => !string.IsNullOrEmpty(w.Country));

        RuleFor(w => w.ContactName)
            .MaximumLength(100).WithMessage("Contact Name must not exceed 100 characters")
            .When(w => !string.IsNullOrEmpty(w.ContactName));

        RuleFor(w => w.ContactPhone)
            .MaximumLength(20).WithMessage("Contact Phone must not exceed 20 characters")
            .When(w => !string.IsNullOrEmpty(w.ContactPhone));
    }
}
