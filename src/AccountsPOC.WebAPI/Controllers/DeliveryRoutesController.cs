using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryRoutesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DeliveryRoutesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryRoute>>> GetDeliveryRoutes([FromQuery] string? status = null, [FromQuery] DateTime? routeDate = null)
    {
        var query = _context.DeliveryRoutes
            .Include(d => d.Driver)
            .Include(d => d.Stops)
                .ThenInclude(s => s.Customer)
            .Include(d => d.Stops)
                .ThenInclude(s => s.SalesOrder)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(d => d.Status == status);
        }

        if (routeDate.HasValue)
        {
            query = query.Where(d => d.RouteDate.Date == routeDate.Value.Date);
        }

        return await query.OrderByDescending(d => d.RouteDate).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryRoute>> GetDeliveryRoute(int id)
    {
        var deliveryRoute = await _context.DeliveryRoutes
            .Include(d => d.Driver)
            .Include(d => d.Stops)
                .ThenInclude(s => s.Customer)
            .Include(d => d.Stops)
                .ThenInclude(s => s.SalesOrder)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deliveryRoute == null)
        {
            return NotFound();
        }

        return deliveryRoute;
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryRoute>> PostDeliveryRoute(DeliveryRoute deliveryRoute)
    {
        deliveryRoute.CreatedDate = DateTime.UtcNow;
        _context.DeliveryRoutes.Add(deliveryRoute);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDeliveryRoute), new { id = deliveryRoute.Id }, deliveryRoute);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutDeliveryRoute(int id, DeliveryRoute deliveryRoute)
    {
        if (id != deliveryRoute.Id)
        {
            return BadRequest();
        }

        deliveryRoute.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(deliveryRoute).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.DeliveryRoutes.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpPatch("{id}/start")]
    public async Task<IActionResult> StartDeliveryRoute(int id)
    {
        var deliveryRoute = await _context.DeliveryRoutes.FindAsync(id);
        if (deliveryRoute == null)
        {
            return NotFound();
        }

        deliveryRoute.Status = "InProgress";
        deliveryRoute.StartedDate = DateTime.UtcNow;
        deliveryRoute.LastModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> CompleteDeliveryRoute(int id)
    {
        var deliveryRoute = await _context.DeliveryRoutes.FindAsync(id);
        if (deliveryRoute == null)
        {
            return NotFound();
        }

        deliveryRoute.Status = "Completed";
        deliveryRoute.CompletedDate = DateTime.UtcNow;
        deliveryRoute.LastModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("stops/{stopId}/update-contact")]
    public async Task<IActionResult> UpdateStopContact(int stopId, [FromBody] UpdateContactRequest request)
    {
        var stop = await _context.DeliveryStops.FindAsync(stopId);
        if (stop == null)
        {
            return NotFound();
        }

        stop.ContactPhone = request.ContactPhone;
        stop.ContactEmail = request.ContactEmail;
        stop.ContactName = request.ContactName;
        
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("stops/{stopId}/capture-evidence")]
    public async Task<IActionResult> CaptureEvidence(int stopId, [FromBody] CaptureEvidenceRequest request)
    {
        var stop = await _context.DeliveryStops.FindAsync(stopId);
        if (stop == null)
        {
            return NotFound();
        }

        stop.SignatureImagePath = request.SignatureImagePath;
        stop.PhotoEvidencePaths = request.PhotoEvidencePaths;
        stop.EvidenceCaptured = true;
        stop.DeliveryTime = DateTime.UtcNow;
        stop.Status = "Delivered";
        
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeliveryRoute(int id)
    {
        var deliveryRoute = await _context.DeliveryRoutes.FindAsync(id);
        if (deliveryRoute == null)
        {
            return NotFound();
        }

        _context.DeliveryRoutes.Remove(deliveryRoute);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
