using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankAccountsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BankAccountsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
    {
        return await _context.BankAccounts.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BankAccount>> GetBankAccount(int id)
    {
        var bankAccount = await _context.BankAccounts.FindAsync(id);

        if (bankAccount == null)
        {
            return NotFound();
        }

        return bankAccount;
    }

    [HttpPost]
    public async Task<ActionResult<BankAccount>> PostBankAccount(BankAccount bankAccount)
    {
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

        bankAccount.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(bankAccount).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BankAccountExists(id))
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
        var bankAccount = await _context.BankAccounts.FindAsync(id);
        if (bankAccount == null)
        {
            return NotFound();
        }

        _context.BankAccounts.Remove(bankAccount);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BankAccountExists(int id)
    {
        return _context.BankAccounts.Any(e => e.Id == id);
    }
}
