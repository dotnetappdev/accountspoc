using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstallationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILicenseService _licenseService;

    public InstallationsController(ApplicationDbContext context, ILicenseService licenseService)
    {
        _context = context;
        _licenseService = licenseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Installation>>> GetInstallations()
    {
        return await _context.Installations.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Installation>> GetInstallation(int id)
    {
        var installation = await _context.Installations.FindAsync(id);

        if (installation == null)
        {
            return NotFound();
        }

        return installation;
    }

    [HttpGet("tenant/{tenantId}")]
    public async Task<ActionResult<IEnumerable<Installation>>> GetInstallationsByTenant(int tenantId)
    {
        var installations = await _context.Installations
            .Where(i => i.TenantId == tenantId)
            .OrderByDescending(i => i.ActivationDate)
            .ToListAsync();

        return installations;
    }

    [HttpPost]
    public async Task<ActionResult<Installation>> PostInstallation(CreateInstallationDto dto)
    {
        // Validate license exists and is active
        var license = await _context.Licenses.FindAsync(dto.LicenseId);
        if (license == null || !license.IsActive)
        {
            return BadRequest("Invalid or inactive license.");
        }

        // Check if tenant can add more installations
        if (!await _licenseService.CanAddInstallation(license.TenantId))
        {
            return BadRequest($"Maximum number of installations ({license.MaxInstallations}) reached for this license.");
        }

        // Validate InstallationKey uniqueness
        if (await _context.Installations.AnyAsync(i => i.InstallationKey == dto.InstallationKey))
        {
            return BadRequest("InstallationKey already exists.");
        }

        var installation = new Installation
        {
            TenantId = license.TenantId,
            LicenseId = dto.LicenseId,
            InstallationKey = dto.InstallationKey,
            MachineName = dto.MachineName,
            MachineIdentifier = dto.MachineIdentifier,
            IpAddress = dto.IpAddress,
            Version = dto.Version,
            IsActive = true,
            ActivationDate = DateTime.UtcNow,
            LastHeartbeat = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow
        };

        _context.Installations.Add(installation);
        
        // Update current installations count
        license.CurrentInstallations = await _context.Installations
            .CountAsync(i => i.LicenseId == license.Id && i.IsActive);
        
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInstallation), new { id = installation.Id }, installation);
    }

    [HttpPost("{id}/heartbeat")]
    public async Task<IActionResult> Heartbeat(int id)
    {
        var installation = await _context.Installations.FindAsync(id);
        if (installation == null)
        {
            return NotFound();
        }

        installation.LastHeartbeat = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateInstallation(int id)
    {
        var installation = await _context.Installations.FindAsync(id);
        if (installation == null)
        {
            return NotFound();
        }

        installation.IsActive = false;
        installation.DeactivationDate = DateTime.UtcNow;
        installation.LastModifiedDate = DateTime.UtcNow;

        // Update current installations count
        var license = await _context.Licenses.FindAsync(installation.LicenseId);
        if (license != null)
        {
            license.CurrentInstallations = await _context.Installations
                .CountAsync(i => i.LicenseId == license.Id && i.IsActive);
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInstallation(int id)
    {
        var installation = await _context.Installations.FindAsync(id);
        if (installation == null)
        {
            return NotFound();
        }

        var licenseId = installation.LicenseId;

        _context.Installations.Remove(installation);
        
        // Update current installations count
        var license = await _context.Licenses.FindAsync(licenseId);
        if (license != null)
        {
            license.CurrentInstallations = await _context.Installations
                .CountAsync(i => i.LicenseId == license.Id && i.IsActive);
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateInstallationDto
{
    public int LicenseId { get; set; }
    public required string InstallationKey { get; set; }
    public required string MachineName { get; set; }
    public string? MachineIdentifier { get; set; }
    public string? IpAddress { get; set; }
    public string? Version { get; set; }
}
