using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceListsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PriceListsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PriceList>>> GetPriceLists()
    {
        return await _context.PriceLists
            .Include(p => p.PriceListItems)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PriceList>> GetPriceList(int id)
    {
        var priceList = await _context.PriceLists
            .Include(p => p.PriceListItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (priceList == null)
        {
            return NotFound();
        }

        return priceList;
    }

    [HttpPost]
    public async Task<ActionResult<PriceList>> PostPriceList(PriceList priceList)
    {
        priceList.CreatedDate = DateTime.UtcNow;
        _context.PriceLists.Add(priceList);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPriceList), new { id = priceList.Id }, priceList);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPriceList(int id, PriceList priceList)
    {
        if (id != priceList.Id)
        {
            return BadRequest();
        }

        priceList.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(priceList).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PriceListExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePriceList(int id)
    {
        var priceList = await _context.PriceLists.FindAsync(id);
        if (priceList == null)
        {
            return NotFound();
        }

        _context.PriceLists.Remove(priceList);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PriceListExists(int id)
    {
        return _context.PriceLists.Any(e => e.Id == id);
    }
}
