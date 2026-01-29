using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TenantsController(ApplicationDbContext context)
    {
        _context = context;
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
        tenant.CreatedDate = DateTime.UtcNow;
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
    }

    [HttpPost("CreateWithCustomer")]
    public async Task<ActionResult<object>> CreateTenantWithCustomer(TenantCustomerDto dto)
    {
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
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
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
