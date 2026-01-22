using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiKeyConfigController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private const int DefaultTenantId = 1;

    public ApiKeyConfigController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        // Return masked keys for security
        var configs = await _context.ApiKeyConfigs
            .Where(c => c.TenantId == DefaultTenantId)
            .OrderBy(c => c.ServiceName)
            .ThenBy(c => c.KeyName)
            .Select(c => new
            {
                c.Id,
                c.ServiceName,
                c.KeyName,
                c.KeyType,
                MaskedKeyValue = MaskKey(c.KeyValue),
                c.Description,
                c.Environment,
                c.IsActive,
                c.ExpiryDate,
                c.CreatedDate,
                c.LastModifiedDate,
                c.LastUsedDate
            })
            .ToListAsync();

        return Ok(configs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var config = await _context.ApiKeyConfigs
            .Where(c => c.Id == id && c.TenantId == DefaultTenantId)
            .Select(c => new
            {
                c.Id,
                c.ServiceName,
                c.KeyName,
                c.KeyType,
                MaskedKeyValue = MaskKey(c.KeyValue),
                c.Description,
                c.Environment,
                c.IsActive,
                c.ExpiryDate,
                c.CreatedDate,
                c.LastModifiedDate,
                c.LastUsedDate
            })
            .FirstOrDefaultAsync();

        if (config == null)
            return NotFound();

        return Ok(config);
    }

    [HttpGet("by-service/{serviceName}")]
    public async Task<ActionResult<IEnumerable<object>>> GetByService(string serviceName)
    {
        var configs = await _context.ApiKeyConfigs
            .Where(c => c.ServiceName == serviceName && c.TenantId == DefaultTenantId)
            .Select(c => new
            {
                c.Id,
                c.ServiceName,
                c.KeyName,
                c.KeyType,
                MaskedKeyValue = MaskKey(c.KeyValue),
                c.Description,
                c.Environment,
                c.IsActive,
                c.ExpiryDate,
                c.CreatedDate,
                c.LastModifiedDate,
                c.LastUsedDate
            })
            .ToListAsync();

        return Ok(configs);
    }

    [HttpGet("{id}/reveal")]
    public async Task<ActionResult<string>> RevealKey(int id)
    {
        // In production, this should require additional authentication/authorization
        var config = await _context.ApiKeyConfigs
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == DefaultTenantId);

        if (config == null)
            return NotFound();

        return Ok(new { KeyValue = config.KeyValue });
    }

    [HttpPost]
    public async Task<ActionResult<ApiKeyConfig>> Create(ApiKeyConfig config)
    {
        config.TenantId = DefaultTenantId;
        config.CreatedDate = DateTime.UtcNow;

        _context.ApiKeyConfigs.Add(config);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = config.Id }, new
        {
            config.Id,
            config.ServiceName,
            config.KeyName,
            config.KeyType,
            MaskedKeyValue = MaskKey(config.KeyValue),
            config.Description,
            config.Environment,
            config.IsActive,
            config.ExpiryDate,
            config.CreatedDate
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ApiKeyConfig config)
    {
        if (id != config.Id)
            return BadRequest();

        var existing = await _context.ApiKeyConfigs
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == DefaultTenantId);

        if (existing == null)
            return NotFound();

        existing.ServiceName = config.ServiceName;
        existing.KeyName = config.KeyName;
        existing.KeyType = config.KeyType;
        existing.KeyValue = config.KeyValue;
        existing.Description = config.Description;
        existing.Environment = config.Environment;
        existing.IsActive = config.IsActive;
        existing.ExpiryDate = config.ExpiryDate;
        existing.LastModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var config = await _context.ApiKeyConfigs
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == DefaultTenantId);

        if (config == null)
            return NotFound();

        _context.ApiKeyConfigs.Remove(config);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/rotate")]
    public async Task<ActionResult<object>> RotateKey(int id, [FromBody] string newKeyValue)
    {
        var config = await _context.ApiKeyConfigs
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == DefaultTenantId);

        if (config == null)
            return NotFound();

        config.KeyValue = newKeyValue;
        config.LastModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Success = true,
            Message = "API key rotated successfully",
            MaskedKeyValue = MaskKey(newKeyValue)
        });
    }

    [HttpPost("{id}/mark-used")]
    public async Task<IActionResult> MarkAsUsed(int id)
    {
        var config = await _context.ApiKeyConfigs
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == DefaultTenantId);

        if (config == null)
            return NotFound();

        config.LastUsedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string MaskKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            return string.Empty;

        if (key.Length <= 8)
            return new string('*', key.Length);

        return $"{key.Substring(0, 4)}{'*'.ToString().PadLeft(key.Length - 8, '*')}{key.Substring(key.Length - 4)}";
    }
}
