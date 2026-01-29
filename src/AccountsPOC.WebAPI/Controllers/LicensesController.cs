using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LicensesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILicenseService _licenseService;

    public LicensesController(ApplicationDbContext context, ILicenseService licenseService)
    {
        _context = context;
        _licenseService = licenseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<License>>> GetLicenses()
    {
        return await _context.Licenses.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<License>> GetLicense(int id)
    {
        var license = await _context.Licenses.FindAsync(id);

        if (license == null)
        {
            return NotFound();
        }

        return license;
    }

    [HttpGet("tenant/{tenantId}")]
    public async Task<ActionResult<License>> GetActiveLicenseByTenant(int tenantId)
    {
        var license = await _licenseService.GetActiveLicense(tenantId);

        if (license == null)
        {
            return NotFound("No active license found for this tenant");
        }

        return license;
    }

    [HttpGet("tenant/{tenantId}/validate")]
    public async Task<ActionResult<LicenseValidationResult>> ValidateLicense(int tenantId)
    {
        var result = await _licenseService.ValidateLicense(tenantId);
        return result;
    }

    [HttpGet("tenant/{tenantId}/usage")]
    public async Task<ActionResult<object>> GetLicenseUsage(int tenantId)
    {
        var license = await _licenseService.GetActiveLicense(tenantId);
        if (license == null)
        {
            return NotFound("No active license found");
        }

        var stockItemCount = await _context.StockItems.CountAsync(s => s.TenantId == tenantId);
        var userCount = await _context.Users.CountAsync(u => u.TenantId == tenantId);
        var customerCount = await _context.Customers.CountAsync(c => c.TenantId == tenantId);
        var installationCount = await _context.Installations.CountAsync(i => i.TenantId == tenantId && i.IsActive);

        return Ok(new
        {
            License = license,
            Usage = new
            {
                StockItems = new { Current = stockItemCount, Max = license.MaxStockItems, Percentage = license.MaxStockItems.HasValue ? (stockItemCount * 100.0 / license.MaxStockItems.Value) : 0 },
                Users = new { Current = userCount, Max = license.MaxUsers, Percentage = license.MaxUsers.HasValue ? (userCount * 100.0 / license.MaxUsers.Value) : 0 },
                Customers = new { Current = customerCount, Max = license.MaxCustomers, Percentage = license.MaxCustomers.HasValue ? (customerCount * 100.0 / license.MaxCustomers.Value) : 0 },
                Installations = new { Current = installationCount, Max = license.MaxInstallations, Percentage = (installationCount * 100.0 / license.MaxInstallations) }
            }
        });
    }

    [HttpPost]
    public async Task<ActionResult<License>> PostLicense(CreateLicenseDto dto)
    {
        // Validate TenantId exists
        if (!await _context.Tenants.AnyAsync(t => t.Id == dto.TenantId))
        {
            return BadRequest("Invalid TenantId. Tenant does not exist.");
        }

        // Validate LicenseKey uniqueness
        if (await _context.Licenses.AnyAsync(l => l.LicenseKey == dto.LicenseKey))
        {
            return BadRequest("LicenseKey already exists.");
        }

        var license = new License
        {
            TenantId = dto.TenantId,
            LicenseKey = dto.LicenseKey,
            LicenseType = dto.LicenseType,
            IsActive = dto.IsActive,
            ActivationDate = dto.ActivationDate ?? DateTime.UtcNow,
            ExpiryDate = dto.ExpiryDate,
            MaxInstallations = dto.MaxInstallations,
            MaxStockItems = dto.MaxStockItems,
            AllowMultipleImages = dto.AllowMultipleImages,
            MaxImagesPerStockItem = dto.MaxImagesPerStockItem,
            MaxUsers = dto.MaxUsers,
            MaxRoles = dto.MaxRoles,
            MaxCustomers = dto.MaxCustomers,
            MaxTenants = dto.MaxTenants,
            MaxSalesOrdersPerMonth = dto.MaxSalesOrdersPerMonth,
            MaxPurchaseOrdersPerMonth = dto.MaxPurchaseOrdersPerMonth,
            MaxWarehouses = dto.MaxWarehouses,
            MaxProducts = dto.MaxProducts,
            EnablePdfExport = dto.EnablePdfExport,
            EnableEmailTemplates = dto.EnableEmailTemplates,
            EnableCustomForms = dto.EnableCustomForms,
            EnablePaymentIntegration = dto.EnablePaymentIntegration,
            EnableAdvancedReporting = dto.EnableAdvancedReporting,
            EnableApiAccess = dto.EnableApiAccess,
            EnableMultipleCurrencies = dto.EnableMultipleCurrencies,
            Notes = dto.Notes,
            CreatedDate = DateTime.UtcNow
        };

        _context.Licenses.Add(license);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLicense), new { id = license.Id }, license);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutLicense(int id, UpdateLicenseDto dto)
    {
        var license = await _context.Licenses.FindAsync(id);
        if (license == null)
        {
            return NotFound();
        }

        license.IsActive = dto.IsActive;
        license.ExpiryDate = dto.ExpiryDate;
        license.MaxInstallations = dto.MaxInstallations;
        license.MaxStockItems = dto.MaxStockItems;
        license.AllowMultipleImages = dto.AllowMultipleImages;
        license.MaxImagesPerStockItem = dto.MaxImagesPerStockItem;
        license.MaxUsers = dto.MaxUsers;
        license.MaxRoles = dto.MaxRoles;
        license.MaxCustomers = dto.MaxCustomers;
        license.MaxTenants = dto.MaxTenants;
        license.MaxSalesOrdersPerMonth = dto.MaxSalesOrdersPerMonth;
        license.MaxPurchaseOrdersPerMonth = dto.MaxPurchaseOrdersPerMonth;
        license.MaxWarehouses = dto.MaxWarehouses;
        license.MaxProducts = dto.MaxProducts;
        license.EnablePdfExport = dto.EnablePdfExport;
        license.EnableEmailTemplates = dto.EnableEmailTemplates;
        license.EnableCustomForms = dto.EnableCustomForms;
        license.EnablePaymentIntegration = dto.EnablePaymentIntegration;
        license.EnableAdvancedReporting = dto.EnableAdvancedReporting;
        license.EnableApiAccess = dto.EnableApiAccess;
        license.EnableMultipleCurrencies = dto.EnableMultipleCurrencies;
        license.Notes = dto.Notes;
        license.LastModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLicense(int id)
    {
        var license = await _context.Licenses.FindAsync(id);
        if (license == null)
        {
            return NotFound();
        }

        _context.Licenses.Remove(license);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateLicenseDto
{
    public int TenantId { get; set; }
    public required string LicenseKey { get; set; }
    public required string LicenseType { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ActivationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int MaxInstallations { get; set; } = 1;
    public int? MaxStockItems { get; set; }
    public bool AllowMultipleImages { get; set; } = false;
    public int? MaxImagesPerStockItem { get; set; }
    public int? MaxUsers { get; set; }
    public int? MaxRoles { get; set; }
    public int? MaxCustomers { get; set; }
    public int? MaxTenants { get; set; }
    public int? MaxSalesOrdersPerMonth { get; set; }
    public int? MaxPurchaseOrdersPerMonth { get; set; }
    public int? MaxWarehouses { get; set; }
    public int? MaxProducts { get; set; }
    public bool EnablePdfExport { get; set; } = true;
    public bool EnableEmailTemplates { get; set; } = true;
    public bool EnableCustomForms { get; set; } = false;
    public bool EnablePaymentIntegration { get; set; } = false;
    public bool EnableAdvancedReporting { get; set; } = false;
    public bool EnableApiAccess { get; set; } = false;
    public bool EnableMultipleCurrencies { get; set; } = false;
    public string? Notes { get; set; }
}

public class UpdateLicenseDto
{
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int MaxInstallations { get; set; }
    public int? MaxStockItems { get; set; }
    public bool AllowMultipleImages { get; set; }
    public int? MaxImagesPerStockItem { get; set; }
    public int? MaxUsers { get; set; }
    public int? MaxRoles { get; set; }
    public int? MaxCustomers { get; set; }
    public int? MaxTenants { get; set; }
    public int? MaxSalesOrdersPerMonth { get; set; }
    public int? MaxPurchaseOrdersPerMonth { get; set; }
    public int? MaxWarehouses { get; set; }
    public int? MaxProducts { get; set; }
    public bool EnablePdfExport { get; set; }
    public bool EnableEmailTemplates { get; set; }
    public bool EnableCustomForms { get; set; }
    public bool EnablePaymentIntegration { get; set; }
    public bool EnableAdvancedReporting { get; set; }
    public bool EnableApiAccess { get; set; }
    public bool EnableMultipleCurrencies { get; set; }
    public string? Notes { get; set; }
}
