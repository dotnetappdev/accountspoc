using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private const int DefaultTenantId = 1;

    public PaymentController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("create-payment-intent")]
    public async Task<ActionResult<object>> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        try
        {
            var invoice = await _context.SalesInvoices
                .Where(i => i.Id == request.InvoiceId && i.TenantId == DefaultTenantId)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound(new { error = "Invoice not found" });
            }

            var settings = await GetSystemSettingsAsync();
            
            // TODO: Initialize Stripe with settings.StripeSecretKey
            // var stripeClient = new StripeClient(settings.StripeSecretKey);
            // var paymentIntentService = new PaymentIntentService(stripeClient);
            
            // For now, return mock response
            var paymentIntentId = $"pi_mock_{Guid.NewGuid():N}";
            var clientSecret = $"pi_mock_{Guid.NewGuid():N}_secret";

            var transaction = new PaymentTransaction
            {
                TenantId = DefaultTenantId,
                SalesInvoiceId = invoice.Id,
                PaymentMethod = "Stripe",
                TransactionId = Guid.NewGuid().ToString(),
                Amount = invoice.TotalAmount,
                Currency = request.Currency ?? "USD",
                Status = "Pending",
                CustomerEmail = request.CustomerEmail,
                PaymentIntentId = paymentIntentId,
                TransactionDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                paymentIntentId,
                clientSecret,
                amount = invoice.TotalAmount,
                currency = request.Currency ?? "USD",
                transactionId = transaction.Id
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("process-payment")]
    public async Task<ActionResult<object>> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        try
        {
            var transaction = await _context.PaymentTransactions
                .Where(t => t.Id == request.TransactionId && t.TenantId == DefaultTenantId)
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound(new { error = "Transaction not found" });
            }

            var settings = await GetSystemSettingsAsync();

            // TODO: Process payment based on payment method
            // Stripe, ApplePay, GooglePay integration here
            
            // For now, mock successful payment
            transaction.Status = "Completed";
            transaction.TransactionDate = DateTime.UtcNow;

            var invoice = await _context.SalesInvoices.FindAsync(transaction.SalesInvoiceId);
            if (invoice != null)
            {
                invoice.Status = "Paid";
                invoice.LastModifiedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                transactionId = transaction.Id,
                status = transaction.Status,
                message = "Payment processed successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("status/{transactionId}")]
    public async Task<ActionResult<object>> GetPaymentStatus(int transactionId)
    {
        var transaction = await _context.PaymentTransactions
            .Where(t => t.Id == transactionId && t.TenantId == DefaultTenantId)
            .Include(t => t.SalesInvoice)
            .FirstOrDefaultAsync();

        if (transaction == null)
        {
            return NotFound(new { error = "Transaction not found" });
        }

        return Ok(new
        {
            transactionId = transaction.Id,
            invoiceId = transaction.SalesInvoiceId,
            invoiceNumber = transaction.SalesInvoice?.InvoiceNumber,
            amount = transaction.Amount,
            currency = transaction.Currency,
            status = transaction.Status,
            paymentMethod = transaction.PaymentMethod,
            transactionDate = transaction.TransactionDate,
            failureReason = transaction.FailureReason
        });
    }

    [HttpPost("webhook/stripe")]
    public async Task<IActionResult> StripeWebhook()
    {
        try
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            // TODO: Verify webhook signature with Stripe
            // var stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret);
            
            // Handle different event types
            // switch (stripeEvent.Type)
            // {
            //     case Events.PaymentIntentSucceeded:
            //         // Update transaction status
            //         break;
            //     case Events.PaymentIntentPaymentFailed:
            //         // Update transaction with failure reason
            //         break;
            // }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("generate-payment-link/{invoiceId}")]
    public async Task<ActionResult<object>> GeneratePaymentLink(int invoiceId)
    {
        var invoice = await _context.SalesInvoices
            .Where(i => i.Id == invoiceId && i.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();

        if (invoice == null)
        {
            return NotFound(new { error = "Invoice not found" });
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var paymentLink = $"{baseUrl}/payment/{invoice.Id}";
        
        return Ok(new
        {
            invoiceId = invoice.Id,
            invoiceNumber = invoice.InvoiceNumber,
            paymentLink,
            amount = invoice.TotalAmount,
            status = invoice.Status
        });
    }

    private async Task<SystemSettings> GetSystemSettingsAsync()
    {
        var settings = await _context.SystemSettings
            .Where(s => s.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();

        if (settings == null)
        {
            throw new Exception("System settings not configured");
        }

        return settings;
    }
}

public class CreatePaymentIntentRequest
{
    public int InvoiceId { get; set; }
    public string? Currency { get; set; }
    public string? CustomerEmail { get; set; }
}

public class ProcessPaymentRequest
{
    public int TransactionId { get; set; }
    public string PaymentMethod { get; set; } = "Stripe";
    public string? PaymentMethodId { get; set; }
}
