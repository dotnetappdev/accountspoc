using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentProviderConfigController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private const int DefaultTenantId = 1;

    public PaymentProviderConfigController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentProviderConfig>>> GetAll()
    {
        var configs = await _context.PaymentProviderConfigs
            .Where(c => c.TenantId == DefaultTenantId)
            .OrderBy(c => c.ProviderName)
            .ToListAsync();
        return Ok(configs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentProviderConfig>> GetById(int id)
    {
        var config = await _context.PaymentProviderConfigs
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == DefaultTenantId);

        if (config == null)
            return NotFound();

        return Ok(config);
    }

    [HttpGet("by-provider/{providerCode}")]
    public async Task<ActionResult<PaymentProviderConfig>> GetByProviderCode(string providerCode)
    {
        var config = await _context.PaymentProviderConfigs
            .FirstOrDefaultAsync(c => c.ProviderCode == providerCode && c.TenantId == DefaultTenantId);

        if (config == null)
            return NotFound();

        return Ok(config);
    }

    [HttpGet("enabled")]
    public async Task<ActionResult<IEnumerable<PaymentProviderConfig>>> GetEnabled()
    {
        var configs = await _context.PaymentProviderConfigs
            .Where(c => c.TenantId == DefaultTenantId && c.IsEnabled)
            .OrderBy(c => c.ProviderName)
            .ToListAsync();
        return Ok(configs);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentProviderConfig>> Create(PaymentProviderConfig config)
    {
        config.TenantId = DefaultTenantId;
        config.CreatedDate = DateTime.UtcNow;

        // If this is set as default, unset other defaults
        if (config.IsDefault)
        {
            var existingDefaults = await _context.PaymentProviderConfigs
                .Where(c => c.TenantId == DefaultTenantId && c.IsDefault)
                .ToListAsync();
            
            foreach (var existing in existingDefaults)
            {
                existing.IsDefault = false;
            }
        }

        _context.PaymentProviderConfigs.Add(config);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = config.Id }, config);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PaymentProviderConfig config)
    {
        if (id != config.Id)
            return BadRequest();

        var existing = await _context.PaymentProviderConfigs
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == DefaultTenantId);

        if (existing == null)
            return NotFound();

        existing.ProviderName = config.ProviderName;
        existing.ProviderCode = config.ProviderCode;
        existing.PublishableKey = config.PublishableKey;
        existing.SecretKey = config.SecretKey;
        existing.ApiKey = config.ApiKey;
        existing.MerchantId = config.MerchantId;
        existing.WebhookSecret = config.WebhookSecret;
        existing.Environment = config.Environment;
        existing.IsEnabled = config.IsEnabled;
        existing.IsDefault = config.IsDefault;
        existing.AdditionalConfig = config.AdditionalConfig;
        existing.LastModifiedDate = DateTime.UtcNow;

        // If this is set as default, unset other defaults
        if (config.IsDefault)
        {
            var otherDefaults = await _context.PaymentProviderConfigs
                .Where(c => c.TenantId == DefaultTenantId && c.IsDefault && c.Id != id)
                .ToListAsync();
            
            foreach (var other in otherDefaults)
            {
                other.IsDefault = false;
            }
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var config = await _context.PaymentProviderConfigs
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == DefaultTenantId);

        if (config == null)
            return NotFound();

        _context.PaymentProviderConfigs.Remove(config);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/test")]
    public async Task<ActionResult<object>> TestConnection(int id)
    {
        var config = await _context.PaymentProviderConfigs
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == DefaultTenantId);

        if (config == null)
            return NotFound();

        // In a real implementation, this would test the actual connection to the payment provider
        return Ok(new
        {
            Success = true,
            Message = $"Connection test for {config.ProviderName} successful",
            Timestamp = DateTime.UtcNow
        });
    }
}
