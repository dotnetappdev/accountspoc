using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private const int DefaultTenantId = 1;

    public EmailController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("send-invoice")]
    public async Task<ActionResult<object>> SendInvoiceEmail([FromBody] SendInvoiceEmailRequest request)
    {
        try
        {
            var invoice = await _context.SalesInvoices
                .Where(i => i.Id == request.InvoiceId && i.TenantId == DefaultTenantId)
                .Include(i => i.SalesOrder)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound(new { error = "Invoice not found" });
            }

            var template = await _context.EmailTemplates
                .Where(t => t.TenantId == DefaultTenantId && t.TemplateCode == "INVOICE_CREATED" && t.IsActive)
                .FirstOrDefaultAsync();

            if (template == null)
            {
                return NotFound(new { error = "Email template not found" });
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var paymentLink = request.IncludePaymentLink 
                ? $"{baseUrl}/payment/{invoice.Id}" 
                : string.Empty;

            var emailBody = ProcessTemplate(template.BodyHtml, new Dictionary<string, string>
            {
                { "InvoiceNumber", invoice.InvoiceNumber },
                { "InvoiceDate", invoice.InvoiceDate.ToString("yyyy-MM-dd") },
                { "TotalAmount", invoice.TotalAmount.ToString("N2") },
                { "CustomerName", invoice.SalesOrder?.CustomerName ?? "Customer" },
                { "PaymentLink", paymentLink },
                { "DueDate", invoice.DueDate?.ToString("yyyy-MM-dd") ?? "N/A" }
            });

            var emailSubject = ProcessTemplate(template.Subject, new Dictionary<string, string>
            {
                { "InvoiceNumber", invoice.InvoiceNumber }
            });

            await SendEmailAsync(
                request.ToEmail,
                emailSubject,
                emailBody
            );

            return Ok(new
            {
                success = true,
                message = "Invoice email sent successfully",
                to = request.ToEmail,
                invoiceNumber = invoice.InvoiceNumber
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("send-from-template")]
    public async Task<ActionResult<object>> SendFromTemplate([FromBody] SendFromTemplateRequest request)
    {
        try
        {
            var template = await _context.EmailTemplates
                .Where(t => t.Id == request.TemplateId && t.TenantId == DefaultTenantId && t.IsActive)
                .FirstOrDefaultAsync();

            if (template == null)
            {
                return NotFound(new { error = "Template not found" });
            }

            var emailBody = ProcessTemplate(template.BodyHtml, request.Variables ?? new Dictionary<string, string>());
            var emailSubject = ProcessTemplate(template.Subject, request.Variables ?? new Dictionary<string, string>());

            await SendEmailAsync(
                request.ToEmail,
                emailSubject,
                emailBody
            );

            return Ok(new
            {
                success = true,
                message = "Email sent successfully",
                to = request.ToEmail,
                templateCode = template.TemplateCode
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("test-configuration")]
    public async Task<ActionResult<object>> TestEmailConfiguration([FromBody] TestEmailRequest request)
    {
        try
        {
            var settings = await GetSystemSettingsAsync();

            await SendEmailAsync(
                request.ToEmail,
                "Test Email - AccountsPOC",
                "<html><body><h1>Test Email</h1><p>This is a test email from AccountsPOC system.</p></body></html>"
            );

            return Ok(new
            {
                success = true,
                message = "Test email sent successfully",
                to = request.ToEmail,
                fromAddress = settings.EmailFromAddress
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new 
            { 
                success = false,
                error = ex.Message,
                details = "Please check SMTP configuration in appsettings.json"
            });
        }
    }

    private async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
    {
        var settings = await GetSystemSettingsAsync();
        
        var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        var smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? settings.EmailFromAddress;
        var smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
        var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

        using var smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = enableSsl
        };

        var fromAddress = settings.EmailFromAddress ?? smtpUsername ?? "noreply@accountspoc.com";
        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromAddress, settings.EmailFromName ?? "AccountsPOC"),
            Subject = subject,
            Body = bodyHtml,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage);
    }

    private string ProcessTemplate(string template, Dictionary<string, string> variables)
    {
        var result = template;
        foreach (var variable in variables)
        {
            result = Regex.Replace(
                result, 
                $@"\{{\{{{variable.Key}\}}\}}", 
                variable.Value, 
                RegexOptions.IgnoreCase
            );
        }
        return result;
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

public class SendInvoiceEmailRequest
{
    public int InvoiceId { get; set; }
    public required string ToEmail { get; set; }
    public bool IncludePaymentLink { get; set; } = true;
}

public class SendFromTemplateRequest
{
    public int TemplateId { get; set; }
    public required string ToEmail { get; set; }
    public Dictionary<string, string>? Variables { get; set; }
}

public class TestEmailRequest
{
    public required string ToEmail { get; set; }
}
