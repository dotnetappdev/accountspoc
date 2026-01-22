using FluentValidation;
using AccountsPOC.BlazorApp.Models;

namespace AccountsPOC.BlazorApp.Validators;

public class SalesOrderValidator : AbstractValidator<SalesOrder>
{
    public SalesOrderValidator()
    {
        RuleFor(s => s.OrderNumber)
            .NotEmpty().WithMessage("Order Number is required")
            .MaximumLength(50).WithMessage("Order Number must not exceed 50 characters");

        RuleFor(s => s.CustomerName)
            .NotEmpty().WithMessage("Customer Name is required")
            .MaximumLength(200).WithMessage("Customer Name must not exceed 200 characters");

        RuleFor(s => s.CustomerEmail)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
            .When(s => !string.IsNullOrEmpty(s.CustomerEmail));

        RuleFor(s => s.CustomerPhone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters")
            .When(s => !string.IsNullOrEmpty(s.CustomerPhone));

        RuleFor(s => s.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total Amount must be zero or greater");

        RuleFor(s => s.OrderDate)
            .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("Order Date cannot be in the future");
    }
}
