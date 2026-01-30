using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SiteVisitSignOffsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SiteVisitSignOffsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SiteVisitSignOff>>> GetSiteVisitSignOffs()
    {
        return await _context.SiteVisitSignOffs
            .Include(s => s.WorkOrder)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SiteVisitSignOff>> GetSiteVisitSignOff(int id)
    {
        var signOff = await _context.SiteVisitSignOffs
            .Include(s => s.WorkOrder)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (signOff == null)
        {
            return NotFound();
        }

        return signOff;
    }

    [HttpGet("workorder/{workOrderId}")]
    public async Task<ActionResult<IEnumerable<SiteVisitSignOff>>> GetByWorkOrder(int workOrderId)
    {
        return await _context.SiteVisitSignOffs
            .Where(s => s.WorkOrderId == workOrderId)
            .OrderByDescending(s => s.VisitDate)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<SiteVisitSignOff>> PostSiteVisitSignOff(SiteVisitSignOff signOff)
    {
        signOff.CreatedDate = DateTime.UtcNow;
        signOff.SignedDate = DateTime.UtcNow;
        
        _context.SiteVisitSignOffs.Add(signOff);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSiteVisitSignOff), new { id = signOff.Id }, signOff);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSiteVisitSignOff(int id, SiteVisitSignOff signOff)
    {
        if (id != signOff.Id)
        {
            return BadRequest();
        }

        _context.Entry(signOff).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SiteVisitSignOffExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSiteVisitSignOff(int id)
    {
        var signOff = await _context.SiteVisitSignOffs.FindAsync(id);
        if (signOff == null)
        {
            return NotFound();
        }

        _context.SiteVisitSignOffs.Remove(signOff);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SiteVisitSignOffExists(int id)
    {
        return _context.SiteVisitSignOffs.Any(e => e.Id == id);
    }
}
