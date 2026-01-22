using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesOrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SalesOrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalesOrder>>> GetSalesOrders()
    {
        return await _context.SalesOrders
            .Include(o => o.SalesOrderItems)
            .ThenInclude(i => i.Product)
            .Include(o => o.SalesOrderItems)
            .ThenInclude(i => i.BillOfMaterial)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SalesOrder>> GetSalesOrder(int id)
    {
        var salesOrder = await _context.SalesOrders
            .Include(o => o.SalesOrderItems)
            .ThenInclude(i => i.Product)
            .Include(o => o.SalesOrderItems)
            .ThenInclude(i => i.BillOfMaterial)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (salesOrder == null)
        {
            return NotFound();
        }

        return salesOrder;
    }

    [HttpPost]
    public async Task<ActionResult<SalesOrder>> PostSalesOrder(SalesOrder salesOrder)
    {
        salesOrder.CreatedDate = DateTime.UtcNow;
        salesOrder.OrderDate = DateTime.UtcNow;
        
        foreach (var item in salesOrder.SalesOrderItems)
        {
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }
        
        salesOrder.TotalAmount = salesOrder.SalesOrderItems.Sum(i => i.TotalPrice);
        
        _context.SalesOrders.Add(salesOrder);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSalesOrder), new { id = salesOrder.Id }, salesOrder);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSalesOrder(int id, SalesOrder salesOrder)
    {
        if (id != salesOrder.Id)
        {
            return BadRequest();
        }

        salesOrder.LastModifiedDate = DateTime.UtcNow;
        
        foreach (var item in salesOrder.SalesOrderItems)
        {
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }
        
        salesOrder.TotalAmount = salesOrder.SalesOrderItems.Sum(i => i.TotalPrice);
        
        _context.Entry(salesOrder).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SalesOrderExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSalesOrder(int id)
    {
        var salesOrder = await _context.SalesOrders.FindAsync(id);
        if (salesOrder == null)
        {
            return NotFound();
        }

        _context.SalesOrders.Remove(salesOrder);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SalesOrderExists(int id)
    {
        return _context.SalesOrders.Any(e => e.Id == id);
    }
}
