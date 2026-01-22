using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StockItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockItem>>> GetStockItems()
    {
        return await _context.StockItems
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockItem>> GetStockItem(int id)
    {
        var stockItem = await _context.StockItems
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (stockItem == null)
        {
            return NotFound();
        }

        return stockItem;
    }

    [HttpPost]
    public async Task<ActionResult<StockItem>> PostStockItem(StockItem stockItem)
    {
        stockItem.CreatedDate = DateTime.UtcNow;
        _context.StockItems.Add(stockItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStockItem), new { id = stockItem.Id }, stockItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutStockItem(int id, StockItem stockItem)
    {
        if (id != stockItem.Id)
        {
            return BadRequest();
        }

        stockItem.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(stockItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StockItemExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStockItem(int id)
    {
        var stockItem = await _context.StockItems.FindAsync(id);
        if (stockItem == null)
        {
            return NotFound();
        }

        _context.StockItems.Remove(stockItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool StockItemExists(int id)
    {
        return _context.StockItems.Any(e => e.Id == id);
    }
}
