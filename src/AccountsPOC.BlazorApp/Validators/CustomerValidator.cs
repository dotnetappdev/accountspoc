using FluentValidation;
using AccountsPOC.BlazorApp.Models;

namespace AccountsPOC.BlazorApp.Validators;

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(c => c.CustomerCode)
            .NotEmpty().WithMessage("Customer Code is required")
            .MaximumLength(20).WithMessage("Customer Code must not exceed 20 characters");

        RuleFor(c => c.CompanyName)
            .NotEmpty().WithMessage("Company Name is required")
            .MaximumLength(200).WithMessage("Company Name must not exceed 200 characters");

        RuleFor(c => c.ContactName)
            .MaximumLength(100).WithMessage("Contact Name must not exceed 100 characters")
            .When(c => !string.IsNullOrEmpty(c.ContactName));

        RuleFor(c => c.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
            .When(c => !string.IsNullOrEmpty(c.Email));

        RuleFor(c => c.Phone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters")
            .When(c => !string.IsNullOrEmpty(c.Phone));

        RuleFor(c => c.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters");

        RuleFor(c => c.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters")
            .When(c => !string.IsNullOrEmpty(c.City));

        RuleFor(c => c.PostCode)
            .MaximumLength(20).WithMessage("PostCode must not exceed 20 characters")
            .When(c => !string.IsNullOrEmpty(c.PostCode));

        RuleFor(c => c.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters")
            .When(c => !string.IsNullOrEmpty(c.Country));

        RuleFor(c => c.VATNumber)
            .MaximumLength(20).WithMessage("VAT Number must not exceed 20 characters")
            .When(c => !string.IsNullOrEmpty(c.VATNumber));

        RuleFor(c => c.CreditLimit)
            .GreaterThanOrEqualTo(0).WithMessage("Credit Limit must be zero or greater");
    }
}
