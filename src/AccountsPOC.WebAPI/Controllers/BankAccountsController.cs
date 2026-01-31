using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[Route("api/[controller]")]
public class BankAccountsController : TenantAwareControllerBase
{
    private readonly ApplicationDbContext _context;

    public BankAccountsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
    {
        var tenantId = GetRequiredTenantId();
        return await _context.BankAccounts
            .Where(b => b.TenantId == tenantId)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BankAccount>> GetBankAccount(int id)
    {
        var tenantId = GetRequiredTenantId();
        var bankAccount = await _context.BankAccounts
            .FirstOrDefaultAsync(b => b.Id == id && b.TenantId == tenantId);

        if (bankAccount == null)
        {
            return NotFound();
        }

        return bankAccount;
    }

    [HttpPost]
    public async Task<ActionResult<BankAccount>> PostBankAccount(BankAccount bankAccount)
    {
        var tenantId = GetRequiredTenantId();
        
        // Ensure the bank account is created for the current tenant
        bankAccount.TenantId = tenantId;
        bankAccount.CreatedDate = DateTime.UtcNow;
        
        _context.BankAccounts.Add(bankAccount);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBankAccount), new { id = bankAccount.Id }, bankAccount);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutBankAccount(int id, BankAccount bankAccount)
    {
        if (id != bankAccount.Id)
        {
            return BadRequest();
        }

        var tenantId = GetRequiredTenantId();
        
        // Verify the bank account belongs to the current tenant
        var existing = await _context.BankAccounts
            .FirstOrDefaultAsync(b => b.Id == id && b.TenantId == tenantId);
        
        if (existing == null)
        {
            return NotFound();
        }

        // Ensure tenant ID cannot be changed
        bankAccount.TenantId = tenantId;
        bankAccount.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(bankAccount).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await BankAccountExistsForTenant(id, tenantId))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBankAccount(int id)
    {
        var tenantId = GetRequiredTenantId();
        var bankAccount = await _context.BankAccounts
            .FirstOrDefaultAsync(b => b.Id == id && b.TenantId == tenantId);
        
        if (bankAccount == null)
        {
            return NotFound();
        }

        _context.BankAccounts.Remove(bankAccount);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> BankAccountExistsForTenant(int id, int tenantId)
    {
        return await _context.BankAccounts.AnyAsync(e => e.Id == id && e.TenantId == tenantId);
    }
}
