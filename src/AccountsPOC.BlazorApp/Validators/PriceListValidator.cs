using FluentValidation;
using AccountsPOC.BlazorApp.Models;

namespace AccountsPOC.BlazorApp.Validators;

public class PriceListValidator : AbstractValidator<PriceList>
{
    public PriceListValidator()
    {
        RuleFor(p => p.PriceListCode)
            .NotEmpty().WithMessage("Price List Code is required")
            .MaximumLength(50).WithMessage("Price List Code must not exceed 50 characters");

        RuleFor(p => p.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(p => p.Currency)
            .MaximumLength(3).WithMessage("Currency must be a 3-letter code (e.g., USD, GBP)")
            .When(p => !string.IsNullOrEmpty(p.Currency));

        RuleFor(p => p.ExpiryDate)
            .GreaterThan(p => p.EffectiveDate).WithMessage("Expiry Date must be after Effective Date")
            .When(p => p.ExpiryDate.HasValue);
    }
}
