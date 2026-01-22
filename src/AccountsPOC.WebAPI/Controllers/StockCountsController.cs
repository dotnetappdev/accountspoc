using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockCountsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StockCountsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockCount>>> GetStockCounts([FromQuery] string? status = null)
    {
        var query = _context.StockCounts
            .Include(s => s.Warehouse)
            .Include(s => s.Items)
                .ThenInclude(i => i.StockItem)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(s => s.Status == status);
        }

        return await query.OrderByDescending(s => s.CreatedDate).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockCount>> GetStockCount(int id)
    {
        var stockCount = await _context.StockCounts
            .Include(s => s.Warehouse)
            .Include(s => s.Items)
                .ThenInclude(i => i.StockItem)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (stockCount == null)
        {
            return NotFound();
        }

        return stockCount;
    }

    [HttpPost]
    public async Task<ActionResult<StockCount>> PostStockCount(StockCount stockCount)
    {
        stockCount.CreatedDate = DateTime.UtcNow;
        _context.StockCounts.Add(stockCount);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStockCount), new { id = stockCount.Id }, stockCount);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutStockCount(int id, StockCount stockCount)
    {
        if (id != stockCount.Id)
        {
            return BadRequest();
        }

        _context.Entry(stockCount).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.StockCounts.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> CompleteStockCount(int id)
    {
        var stockCount = await _context.StockCounts.FindAsync(id);
        if (stockCount == null)
        {
            return NotFound();
        }

        stockCount.Status = "Completed";
        stockCount.CompletedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/reconcile")]
    public async Task<IActionResult> ReconcileStockCount(int id)
    {
        var stockCount = await _context.StockCounts
            .Include(s => s.Items)
                .ThenInclude(i => i.StockItem)
            .FirstOrDefaultAsync(s => s.Id == id);
            
        if (stockCount == null)
        {
            return NotFound();
        }

        // Validate counted quantities are not negative
        var invalidItems = stockCount.Items.Where(i => i.CountedQuantity < 0).ToList();
        if (invalidItems.Any())
        {
            return BadRequest($"Invalid counted quantities for items: {string.Join(", ", invalidItems.Select(i => i.StockItemId))}");
        }

        // Update stock item quantities based on counted quantities
        foreach (var item in stockCount.Items)
        {
            if (item.StockItem != null)
            {
                item.StockItem.QuantityOnHand = item.CountedQuantity;
            }
        }

        stockCount.Status = "Reconciled";
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStockCount(int id)
    {
        var stockCount = await _context.StockCounts.FindAsync(id);
        if (stockCount == null)
        {
            return NotFound();
        }

        _context.StockCounts.Remove(stockCount);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
