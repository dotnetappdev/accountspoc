using FluentValidation;
using AccountsPOC.BlazorApp.Models;

namespace AccountsPOC.BlazorApp.Validators;

public class BankAccountValidator : AbstractValidator<BankAccount>
{
    public BankAccountValidator()
    {
        RuleFor(b => b.AccountName)
            .NotEmpty().WithMessage("Account Name is required")
            .MaximumLength(200).WithMessage("Account Name must not exceed 200 characters");

        RuleFor(b => b.AccountNumber)
            .NotEmpty().WithMessage("Account Number is required")
            .MaximumLength(50).WithMessage("Account Number must not exceed 50 characters");

        RuleFor(b => b.BankName)
            .NotEmpty().WithMessage("Bank Name is required")
            .MaximumLength(200).WithMessage("Bank Name must not exceed 200 characters");

        RuleFor(b => b.BranchCode)
            .MaximumLength(20).WithMessage("Branch Code must not exceed 20 characters")
            .When(b => !string.IsNullOrEmpty(b.BranchCode));

        RuleFor(b => b.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .MaximumLength(3).WithMessage("Currency must be a 3-letter code (e.g., USD, GBP)");
    }
}
