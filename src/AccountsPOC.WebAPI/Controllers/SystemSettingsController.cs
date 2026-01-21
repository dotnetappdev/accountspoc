using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemSettingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private const int DefaultTenantId = 1;

    public SystemSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SystemSettings>>> GetSystemSettings()
    {
        return await _context.SystemSettings
            .Where(s => s.TenantId == DefaultTenantId)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SystemSettings>> GetSystemSettings(int id)
    {
        var systemSettings = await _context.SystemSettings
            .Where(s => s.Id == id && s.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();

        if (systemSettings == null)
        {
            return NotFound();
        }

        return systemSettings;
    }

    [HttpPost]
    public async Task<ActionResult<SystemSettings>> PostSystemSettings(SystemSettings systemSettings)
    {
        systemSettings.TenantId = DefaultTenantId;
        systemSettings.CreatedDate = DateTime.UtcNow;
        
        _context.SystemSettings.Add(systemSettings);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSystemSettings), new { id = systemSettings.Id }, systemSettings);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSystemSettings(int id, SystemSettings systemSettings)
    {
        if (id != systemSettings.Id)
        {
            return BadRequest();
        }

        var existingSettings = await _context.SystemSettings
            .Where(s => s.Id == id && s.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();

        if (existingSettings == null)
        {
            return NotFound();
        }

        systemSettings.TenantId = DefaultTenantId;
        systemSettings.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(existingSettings).CurrentValues.SetValues(systemSettings);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SystemSettingsExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSystemSettings(int id)
    {
        var systemSettings = await _context.SystemSettings
            .Where(s => s.Id == id && s.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();
            
        if (systemSettings == null)
        {
            return NotFound();
        }

        _context.SystemSettings.Remove(systemSettings);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SystemSettingsExists(int id)
    {
        return _context.SystemSettings.Any(e => e.Id == id && e.TenantId == DefaultTenantId);
    }
}
