using FluentValidation;
using AccountsPOC.BlazorApp.Models;

namespace AccountsPOC.BlazorApp.Validators;

public class BillOfMaterialValidator : AbstractValidator<BillOfMaterial>
{
    public BillOfMaterialValidator()
    {
        RuleFor(b => b.BOMNumber)
            .NotEmpty().WithMessage("BOM Number is required")
            .MaximumLength(50).WithMessage("BOM Number must not exceed 50 characters");

        RuleFor(b => b.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(b => b.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(b => !string.IsNullOrEmpty(b.Description));

        RuleFor(b => b.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(b => b.EstimatedCost)
            .GreaterThanOrEqualTo(0).WithMessage("Estimated Cost must be zero or greater");
    }
}
