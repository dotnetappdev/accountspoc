using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomFieldsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private const int DefaultTenantId = 1;

    public CustomFieldsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomField>>> GetCustomFields()
    {
        return await _context.CustomFields
            .Where(c => c.TenantId == DefaultTenantId)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomField>> GetCustomField(int id)
    {
        var customField = await _context.CustomFields
            .Where(c => c.Id == id && c.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();

        if (customField == null)
        {
            return NotFound();
        }

        return customField;
    }

    [HttpGet("by-entity/{entityType}")]
    public async Task<ActionResult<IEnumerable<CustomField>>> GetCustomFieldsByEntityType(string entityType)
    {
        return await _context.CustomFields
            .Where(c => c.TenantId == DefaultTenantId && c.EntityType == entityType && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<CustomField>> PostCustomField(CustomField customField)
    {
        customField.TenantId = DefaultTenantId;
        customField.CreatedDate = DateTime.UtcNow;
        
        _context.CustomFields.Add(customField);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCustomField), new { id = customField.Id }, customField);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomField(int id, CustomField customField)
    {
        if (id != customField.Id)
        {
            return BadRequest();
        }

        var existingField = await _context.CustomFields
            .Where(c => c.Id == id && c.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();

        if (existingField == null)
        {
            return NotFound();
        }

        customField.TenantId = DefaultTenantId;
        _context.Entry(existingField).CurrentValues.SetValues(customField);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CustomFieldExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomField(int id)
    {
        var customField = await _context.CustomFields
            .Where(c => c.Id == id && c.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();
            
        if (customField == null)
        {
            return NotFound();
        }

        _context.CustomFields.Remove(customField);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CustomFieldExists(int id)
    {
        return _context.CustomFields.Any(e => e.Id == id && e.TenantId == DefaultTenantId);
    }
}
