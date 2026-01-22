using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PickListsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PickListsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PickList>>> GetPickLists([FromQuery] string? status = null)
    {
        var query = _context.PickLists
            .Include(p => p.SalesOrder)
            .Include(p => p.Items)
                .ThenInclude(i => i.StockItem)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(p => p.Status == status);
        }

        return await query.OrderByDescending(p => p.CreatedDate).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PickList>> GetPickList(int id)
    {
        var pickList = await _context.PickLists
            .Include(p => p.SalesOrder)
            .Include(p => p.Items)
                .ThenInclude(i => i.StockItem)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pickList == null)
        {
            return NotFound();
        }

        return pickList;
    }

    [HttpPost]
    public async Task<ActionResult<PickList>> PostPickList(PickList pickList)
    {
        pickList.CreatedDate = DateTime.UtcNow;
        _context.PickLists.Add(pickList);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPickList), new { id = pickList.Id }, pickList);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPickList(int id, PickList pickList)
    {
        if (id != pickList.Id)
        {
            return BadRequest();
        }

        pickList.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(pickList).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.PickLists.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpPatch("{id}/start")]
    public async Task<IActionResult> StartPickList(int id)
    {
        var pickList = await _context.PickLists.FindAsync(id);
        if (pickList == null)
        {
            return NotFound();
        }

        pickList.Status = "InProgress";
        pickList.StartedDate = DateTime.UtcNow;
        pickList.LastModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> CompletePickList(int id)
    {
        var pickList = await _context.PickLists.FindAsync(id);
        if (pickList == null)
        {
            return NotFound();
        }

        pickList.Status = "Completed";
        pickList.CompletedDate = DateTime.UtcNow;
        pickList.LastModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePickList(int id)
    {
        var pickList = await _context.PickLists.FindAsync(id);
        if (pickList == null)
        {
            return NotFound();
        }

        _context.PickLists.Remove(pickList);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
