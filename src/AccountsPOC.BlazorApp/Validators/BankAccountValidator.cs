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

        RuleFor(b => b.IBAN)
            .MaximumLength(34).WithMessage("IBAN must not exceed 34 characters")
            .Matches("^[A-Z]{2}[0-9]{2}[A-Z0-9]+$").WithMessage("IBAN format is invalid (e.g., GB29NWBK60161331926819)")
            .When(b => !string.IsNullOrEmpty(b.IBAN));

        RuleFor(b => b.SortCode)
            .MaximumLength(10).WithMessage("Sort Code must not exceed 10 characters")
            .Matches("^[0-9-]+$").WithMessage("Sort Code should contain only numbers and hyphens")
            .When(b => !string.IsNullOrEmpty(b.SortCode));

        RuleFor(b => b.AccountCode)
            .MaximumLength(50).WithMessage("Account Code must not exceed 50 characters")
            .When(b => !string.IsNullOrEmpty(b.AccountCode));

        RuleFor(b => b.IssueCode)
            .MaximumLength(10).WithMessage("Issue Code must not exceed 10 characters")
            .When(b => !string.IsNullOrEmpty(b.IssueCode));

        RuleFor(b => b.ExpiryCode)
            .MaximumLength(10).WithMessage("Expiry Code must not exceed 10 characters")
            .When(b => !string.IsNullOrEmpty(b.ExpiryCode));

        RuleFor(b => b.StartCode)
            .MaximumLength(10).WithMessage("Start Code must not exceed 10 characters")
            .When(b => !string.IsNullOrEmpty(b.StartCode));

        RuleFor(b => b.Extra)
            .MaximumLength(500).WithMessage("Extra information must not exceed 500 characters")
            .When(b => !string.IsNullOrEmpty(b.Extra));

        RuleFor(b => b.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .MaximumLength(3).WithMessage("Currency must be a 3-letter code (e.g., USD, GBP)");
    }
}
