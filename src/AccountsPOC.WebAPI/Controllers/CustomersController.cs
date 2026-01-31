using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[Route("api/[controller]")]
public class CustomersController : TenantAwareControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        var tenantId = GetRequiredTenantId();
        return await _context.Customers
            .Where(c => c.TenantId == tenantId)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var tenantId = GetRequiredTenantId();
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (customer == null)
        {
            return NotFound();
        }

        return customer;
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
    {
        var tenantId = GetRequiredTenantId();
        
        // Ensure the customer is created for the current tenant
        customer.TenantId = tenantId;
        customer.CreatedDate = DateTime.UtcNow;
        
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomer(int id, Customer customer)
    {
        if (id != customer.Id)
        {
            return BadRequest();
        }

        var tenantId = GetRequiredTenantId();
        
        // Verify the customer belongs to the current tenant
        var existing = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
        
        if (existing == null)
        {
            return NotFound();
        }

        // Ensure tenant ID cannot be changed
        customer.TenantId = tenantId;
        customer.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(customer).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CustomerExistsForTenant(id, tenantId))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var tenantId = GetRequiredTenantId();
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
        
        if (customer == null)
        {
            return NotFound();
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Agent-specific endpoint - limited customer data update
    [HttpPatch("{id}/agent-update")]
    public async Task<IActionResult> AgentUpdateCustomer(int id, AgentCustomerUpdateDto dto)
    {
        var tenantId = GetRequiredTenantId();
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
        
        if (customer == null)
        {
            return NotFound();
        }

        // Agents can only update contact and delivery information
        if (dto.ContactName != null) customer.ContactName = dto.ContactName;
        if (dto.Phone != null) customer.Phone = dto.Phone;
        if (dto.Mobile != null) customer.Mobile = dto.Mobile;
        if (dto.Email != null) customer.Email = dto.Email;
        
        // Delivery details
        if (dto.DeliveryContactName != null) customer.DeliveryContactName = dto.DeliveryContactName;
        if (dto.DeliveryContactPhone != null) customer.DeliveryContactPhone = dto.DeliveryContactPhone;
        if (dto.DeliveryContactMobile != null) customer.DeliveryContactMobile = dto.DeliveryContactMobile;
        if (dto.DeliveryInstructions != null) customer.DeliveryInstructions = dto.DeliveryInstructions;
        if (dto.PreferredDeliveryTime != null) customer.PreferredDeliveryTime = dto.PreferredDeliveryTime;
        if (dto.AccessCode != null) customer.AccessCode = dto.AccessCode;
        if (dto.Notes != null) customer.Notes = dto.Notes;

        customer.LastModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> CustomerExistsForTenant(int id, int tenantId)
    {
        return await _context.Customers.AnyAsync(e => e.Id == id && e.TenantId == tenantId);
    }
}

public class AgentCustomerUpdateDto
{
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? DeliveryContactName { get; set; }
    public string? DeliveryContactPhone { get; set; }
    public string? DeliveryContactMobile { get; set; }
    public string? DeliveryInstructions { get; set; }
    public string? PreferredDeliveryTime { get; set; }
    public string? AccessCode { get; set; }
    public string? Notes { get; set; }
}
