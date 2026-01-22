using FluentValidation;
using AccountsPOC.BlazorApp.Models;

namespace AccountsPOC.BlazorApp.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(p => p.ProductCode)
            .NotEmpty().WithMessage("Product Code is required")
            .MaximumLength(50).WithMessage("Product Code must not exceed 50 characters");

        RuleFor(p => p.ProductName)
            .NotEmpty().WithMessage("Product Name is required")
            .MaximumLength(200).WithMessage("Product Name must not exceed 200 characters");

        RuleFor(p => p.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(p => !string.IsNullOrEmpty(p.Description));

        RuleFor(p => p.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit Price must be zero or greater");

        RuleFor(p => p.StockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Stock Level must be zero or greater");

        RuleFor(p => p.ReorderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder Level must be zero or greater");
    }
}
