using FluentValidation;
using AccountsPOC.BlazorApp.Models;

namespace AccountsPOC.BlazorApp.Validators;

public class StockItemValidator : AbstractValidator<StockItem>
{
    public StockItemValidator()
    {
        RuleFor(s => s.StockCode)
            .NotEmpty().WithMessage("Stock Code is required")
            .MaximumLength(50).WithMessage("Stock Code must not exceed 50 characters");

        RuleFor(s => s.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(200).WithMessage("Description must not exceed 200 characters");

        RuleFor(s => s.LongDescription)
            .MaximumLength(1000).WithMessage("Long Description must not exceed 1000 characters")
            .When(s => !string.IsNullOrEmpty(s.LongDescription));

        RuleFor(s => s.CostPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Cost Price must be zero or greater");

        RuleFor(s => s.SellingPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Selling Price must be zero or greater");

        RuleFor(s => s.QuantityOnHand)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity On Hand must be zero or greater");

        RuleFor(s => s.QuantityAllocated)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity Allocated must be zero or greater");

        RuleFor(s => s.ReorderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder Level must be zero or greater");

        RuleFor(s => s.ReorderQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder Quantity must be zero or greater");

        RuleFor(s => s.BinLocation)
            .MaximumLength(50).WithMessage("Bin Location must not exceed 50 characters")
            .When(s => !string.IsNullOrEmpty(s.BinLocation));
    }
}
