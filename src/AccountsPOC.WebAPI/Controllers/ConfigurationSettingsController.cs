using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationSettingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private const int DefaultTenantId = 1;

    public ConfigurationSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/ConfigurationSettings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConfigurationSetting>>> GetConfigurationSettings([FromQuery] string? category = null)
    {
        var query = _context.ConfigurationSettings
            .Where(s => s.TenantId == DefaultTenantId);

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(s => s.Category == category);
        }

        return await query
            .OrderBy(s => s.Category)
            .ThenBy(s => s.DisplayOrder)
            .ThenBy(s => s.Key)
            .ToListAsync();
    }

    // GET: api/ConfigurationSettings/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ConfigurationSetting>> GetConfigurationSetting(int id)
    {
        var setting = await _context.ConfigurationSettings
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == DefaultTenantId);

        if (setting == null)
        {
            return NotFound();
        }

        return setting;
    }

    // GET: api/ConfigurationSettings/by-key/{category}/{key}
    [HttpGet("by-key/{category}/{key}")]
    public async Task<ActionResult<ConfigurationSetting>> GetByKey(string category, string key)
    {
        var setting = await _context.ConfigurationSettings
            .FirstOrDefaultAsync(s => s.TenantId == DefaultTenantId && 
                                    s.Category == category && 
                                    s.Key == key);

        if (setting == null)
        {
            return NotFound();
        }

        return setting;
    }

    // GET: api/ConfigurationSettings/categories
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        var categories = await _context.ConfigurationSettings
            .Where(s => s.TenantId == DefaultTenantId)
            .Select(s => s.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return categories;
    }

    // GET: api/ConfigurationSettings/by-category/{category}
    [HttpGet("by-category/{category}")]
    public async Task<ActionResult<Dictionary<string, string>>> GetByCategory(string category)
    {
        var settings = await _context.ConfigurationSettings
            .Where(s => s.TenantId == DefaultTenantId && s.Category == category)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Key)
            .ToListAsync();

        var result = settings.ToDictionary(s => s.Key, s => s.Value);
        return result;
    }

    // POST: api/ConfigurationSettings
    [HttpPost]
    public async Task<ActionResult<ConfigurationSetting>> PostConfigurationSetting(ConfigurationSetting setting)
    {
        setting.TenantId = DefaultTenantId;
        setting.CreatedDate = DateTime.UtcNow;

        // Check if setting with same category and key already exists
        var existing = await _context.ConfigurationSettings
            .FirstOrDefaultAsync(s => s.TenantId == DefaultTenantId && 
                                    s.Category == setting.Category && 
                                    s.Key == setting.Key);

        if (existing != null)
        {
            return Conflict("A setting with this category and key already exists.");
        }

        _context.ConfigurationSettings.Add(setting);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetConfigurationSetting), new { id = setting.Id }, setting);
    }

    // POST: api/ConfigurationSettings/bulk
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<ConfigurationSetting>>> PostBulkSettings(IEnumerable<ConfigurationSetting> settings)
    {
        var settingsList = settings.ToList();
        
        foreach (var setting in settingsList)
        {
            setting.TenantId = DefaultTenantId;
            setting.CreatedDate = DateTime.UtcNow;
        }

        _context.ConfigurationSettings.AddRange(settingsList);
        await _context.SaveChangesAsync();

        return Ok(settingsList);
    }

    // PUT: api/ConfigurationSettings/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutConfigurationSetting(int id, ConfigurationSetting setting)
    {
        if (id != setting.Id)
        {
            return BadRequest();
        }

        var existingSetting = await _context.ConfigurationSettings
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == DefaultTenantId);

        if (existingSetting == null)
        {
            return NotFound();
        }

        // Check if trying to update to duplicate category/key
        var duplicate = await _context.ConfigurationSettings
            .AnyAsync(s => s.Id != id && 
                        s.TenantId == DefaultTenantId && 
                        s.Category == setting.Category && 
                        s.Key == setting.Key);

        if (duplicate)
        {
            return Conflict("A setting with this category and key already exists.");
        }

        existingSetting.Category = setting.Category;
        existingSetting.Key = setting.Key;
        existingSetting.Value = setting.Value;
        existingSetting.DataType = setting.DataType;
        existingSetting.Description = setting.Description;
        existingSetting.IsEncrypted = setting.IsEncrypted;
        existingSetting.IsSystem = setting.IsSystem;
        existingSetting.DisplayOrder = setting.DisplayOrder;
        existingSetting.ValidationRule = setting.ValidationRule;
        existingSetting.DefaultValue = setting.DefaultValue;
        existingSetting.LastModifiedDate = DateTime.UtcNow;
        existingSetting.LastModifiedBy = setting.LastModifiedBy;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ConfigurationSettingExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // PUT: api/ConfigurationSettings/by-key/{category}/{key}
    [HttpPut("by-key/{category}/{key}")]
    public async Task<IActionResult> PutByKey(string category, string key, [FromBody] string value)
    {
        var setting = await _context.ConfigurationSettings
            .FirstOrDefaultAsync(s => s.TenantId == DefaultTenantId && 
                                    s.Category == category && 
                                    s.Key == key);

        if (setting == null)
        {
            return NotFound();
        }

        setting.Value = value;
        setting.LastModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/ConfigurationSettings/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConfigurationSetting(int id)
    {
        var setting = await _context.ConfigurationSettings
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == DefaultTenantId);

        if (setting == null)
        {
            return NotFound();
        }

        if (setting.IsSystem)
        {
            return BadRequest("System settings cannot be deleted.");
        }

        _context.ConfigurationSettings.Remove(setting);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/ConfigurationSettings/{id}/reset
    [HttpPost("{id}/reset")]
    public async Task<IActionResult> ResetToDefault(int id)
    {
        var setting = await _context.ConfigurationSettings
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == DefaultTenantId);

        if (setting == null)
        {
            return NotFound();
        }

        if (string.IsNullOrEmpty(setting.DefaultValue))
        {
            return BadRequest("This setting does not have a default value.");
        }

        setting.Value = setting.DefaultValue;
        setting.LastModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ConfigurationSettingExists(int id)
    {
        return _context.ConfigurationSettings.Any(e => e.Id == id && e.TenantId == DefaultTenantId);
    }
}
