using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesInvoicesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SalesInvoicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalesInvoice>>> GetSalesInvoices()
    {
        return await _context.SalesInvoices
            .Include(i => i.SalesOrder)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SalesInvoice>> GetSalesInvoice(int id)
    {
        var salesInvoice = await _context.SalesInvoices
            .Include(i => i.SalesOrder)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (salesInvoice == null)
        {
            return NotFound();
        }

        return salesInvoice;
    }

    [HttpPost]
    public async Task<ActionResult<SalesInvoice>> PostSalesInvoice(SalesInvoice salesInvoice)
    {
        salesInvoice.CreatedDate = DateTime.UtcNow;
        salesInvoice.InvoiceDate = DateTime.UtcNow;
        salesInvoice.DueDate = DateTime.UtcNow.AddDays(30);
        
        _context.SalesInvoices.Add(salesInvoice);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSalesInvoice), new { id = salesInvoice.Id }, salesInvoice);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSalesInvoice(int id, SalesInvoice salesInvoice)
    {
        if (id != salesInvoice.Id)
        {
            return BadRequest();
        }

        salesInvoice.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(salesInvoice).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SalesInvoiceExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSalesInvoice(int id)
    {
        var salesInvoice = await _context.SalesInvoices.FindAsync(id);
        if (salesInvoice == null)
        {
            return NotFound();
        }

        _context.SalesInvoices.Remove(salesInvoice);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SalesInvoiceExists(int id)
    {
        return _context.SalesInvoices.Any(e => e.Id == id);
    }
}
