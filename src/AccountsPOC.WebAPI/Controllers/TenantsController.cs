using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILicenseService _licenseService;

    public TenantsController(ApplicationDbContext context, ILicenseService licenseService)
    {
        _context = context;
        _licenseService = licenseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants()
    {
        return await _context.Tenants.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Tenant>> GetTenant(int id)
    {
        var tenant = await _context.Tenants.FindAsync(id);

        if (tenant == null)
        {
            return NotFound();
        }

        return tenant;
    }

    [HttpPost]
    public async Task<ActionResult<Tenant>> PostTenant(Tenant tenant)
    {
        // Check license limit for tenants
        // For a multi-tenant system, typically the master/system tenant controls tenant creation
        var masterTenantId = 1; // TODO: Get from configuration or system tenant
        var canAdd = await _licenseService.CanAddTenant(masterTenantId);
        if (!canAdd)
        {
            return BadRequest("Tenant limit reached for current license. Please upgrade your license to add more tenants.");
        }

        // Validate TenantCode uniqueness
        if (await _context.Tenants.AnyAsync(t => t.TenantCode == tenant.TenantCode))
        {
            return BadRequest("TenantCode already exists.");
        }

        tenant.CreatedDate = DateTime.UtcNow;
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
    }

    [HttpPost("CreateWithCustomer")]
    public async Task<ActionResult<object>> CreateTenantWithCustomer(TenantCustomerDto dto)
    {
        // Check license limit for tenants
        var masterTenantId = 1; // TODO: Get from configuration or system tenant
        var canAdd = await _licenseService.CanAddTenant(masterTenantId);
        if (!canAdd)
        {
            return BadRequest("Tenant limit reached for current license. Please upgrade your license to add more tenants.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Create tenant
            var tenant = new Tenant
            {
                TenantCode = dto.TenantCode,
                CompanyName = dto.CompanyName,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            // Create customer linked to tenant
            var customer = new Customer
            {
                TenantId = tenant.Id,
                CustomerCode = dto.CustomerCode ?? dto.TenantCode,
                CompanyName = dto.CompanyName,
                ContactName = dto.PrimaryContactName,
                Email = dto.ContactEmail,
                Phone = dto.ContactPhone,
                Mobile = dto.Mobile,
                Address = dto.Address,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                County = dto.County,
                PostCode = dto.PostCode,
                Country = dto.Country,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(new { Tenant = tenant, Customer = customer });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating tenant with customer: {ex.Message}");
            return StatusCode(500, new { error = "Failed to create tenant with customer", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTenant(int id, Tenant tenant)
    {
        if (id != tenant.Id)
        {
            return BadRequest();
        }

        tenant.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(tenant).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TenantExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTenant(int id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null)
        {
            return NotFound();
        }

        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TenantExists(int id)
    {
        return _context.Tenants.Any(e => e.Id == id);
    }
}

public class TenantCustomerDto
{
    public required string TenantCode { get; set; }
    public required string CompanyName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? CustomerCode { get; set; }
    public string? PrimaryContactName { get; set; }
    public string? Mobile { get; set; }
    public required string Address { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? County { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
}
