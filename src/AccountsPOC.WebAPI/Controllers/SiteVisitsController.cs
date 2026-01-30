using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SiteVisitsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SiteVisitsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SiteVisit>>> GetSiteVisits()
    {
        return await _context.SiteVisits
            .Include(s => s.Customer)
            .Include(s => s.AssignedTo)
            .Include(s => s.WorkOrders)
            .Include(s => s.SiteVisitSignOffs)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SiteVisit>> GetSiteVisit(int id)
    {
        var siteVisit = await _context.SiteVisits
            .Include(s => s.Customer)
            .Include(s => s.AssignedTo)
            .Include(s => s.WorkOrders)
            .Include(s => s.SiteVisitSignOffs)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (siteVisit == null)
        {
            return NotFound();
        }

        return siteVisit;
    }

    [HttpPost]
    public async Task<ActionResult<SiteVisit>> PostSiteVisit(SiteVisit siteVisit)
    {
        siteVisit.CreatedDate = DateTime.UtcNow;
        
        _context.SiteVisits.Add(siteVisit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSiteVisit), new { id = siteVisit.Id }, siteVisit);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSiteVisit(int id, SiteVisit siteVisit)
    {
        if (id != siteVisit.Id)
        {
            return BadRequest();
        }

        siteVisit.LastModifiedDate = DateTime.UtcNow;
        
        _context.Entry(siteVisit).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SiteVisitExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSiteVisit(int id)
    {
        var siteVisit = await _context.SiteVisits.FindAsync(id);
        if (siteVisit == null)
        {
            return NotFound();
        }

        _context.SiteVisits.Remove(siteVisit);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/assign-workorder/{workOrderId}")]
    public async Task<IActionResult> AssignWorkOrder(int id, int workOrderId)
    {
        var siteVisit = await _context.SiteVisits.FindAsync(id);
        if (siteVisit == null)
        {
            return NotFound("Site visit not found");
        }

        var workOrder = await _context.WorkOrders.FindAsync(workOrderId);
        if (workOrder == null)
        {
            return NotFound("Work order not found");
        }

        workOrder.SiteVisitId = id;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{id}/unassign-workorder/{workOrderId}")]
    public async Task<IActionResult> UnassignWorkOrder(int id, int workOrderId)
    {
        var workOrder = await _context.WorkOrders.FindAsync(workOrderId);
        if (workOrder == null || workOrder.SiteVisitId != id)
        {
            return NotFound();
        }

        workOrder.SiteVisitId = null;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("{id}/workorders")]
    public async Task<ActionResult<IEnumerable<WorkOrder>>> GetWorkOrdersForVisit(int id)
    {
        return await _context.WorkOrders
            .Where(w => w.SiteVisitId == id)
            .Include(w => w.Customer)
            .Include(w => w.WorkOrderTasks)
            .ToListAsync();
    }

    private bool SiteVisitExists(int id)
    {
        return _context.SiteVisits.Any(e => e.Id == id);
    }
}
