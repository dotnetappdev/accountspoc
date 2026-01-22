using FluentValidation;
using AccountsPOC.BlazorApp.Models;

namespace AccountsPOC.BlazorApp.Validators;

public class SalesInvoiceValidator : AbstractValidator<SalesInvoice>
{
    public SalesInvoiceValidator()
    {
        RuleFor(s => s.InvoiceNumber)
            .NotEmpty().WithMessage("Invoice Number is required")
            .MaximumLength(50).WithMessage("Invoice Number must not exceed 50 characters");

        RuleFor(s => s.SubTotal)
            .GreaterThanOrEqualTo(0).WithMessage("Sub Total must be zero or greater");

        RuleFor(s => s.TaxAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Tax Amount must be zero or greater");

        RuleFor(s => s.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total Amount must be zero or greater");

        RuleFor(s => s.InvoiceDate)
            .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("Invoice Date cannot be in the future");

        RuleFor(s => s.DueDate)
            .GreaterThanOrEqualTo(s => s.InvoiceDate).WithMessage("Due Date must be on or after Invoice Date")
            .When(s => s.DueDate.HasValue);
    }
}
