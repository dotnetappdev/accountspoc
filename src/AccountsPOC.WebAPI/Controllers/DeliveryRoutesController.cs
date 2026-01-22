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

    [HttpPost("stops/{stopId}/generate-otp")]
    public async Task<ActionResult<string>> GenerateOTP(int stopId)
    {
        var stop = await _context.DeliveryStops.FindAsync(stopId);
        if (stop == null)
        {
            return NotFound();
        }

        // Generate a 6-digit OTP
        var random = new Random();
        var otp = random.Next(100000, 999999).ToString();
        
        stop.OTPCode = otp;
        stop.OTPGeneratedAt = DateTime.UtcNow;
        stop.OTPVerified = false;
        
        await _context.SaveChangesAsync();

        // In production, this would send SMS/email to customer
        return Ok(new { OTPCode = otp, ExpiresInMinutes = 15 });
    }

    [HttpPost("stops/{stopId}/verify-otp")]
    public async Task<IActionResult> VerifyOTP(int stopId, [FromBody] VerifyOTPRequest request)
    {
        var stop = await _context.DeliveryStops.FindAsync(stopId);
        if (stop == null)
        {
            return NotFound();
        }

        if (string.IsNullOrEmpty(stop.OTPCode))
        {
            return BadRequest("No OTP has been generated for this delivery.");
        }

        // Check if OTP is expired (15 minutes)
        if (stop.OTPGeneratedAt.HasValue && 
            DateTime.UtcNow - stop.OTPGeneratedAt.Value > TimeSpan.FromMinutes(15))
        {
            return BadRequest("OTP has expired. Please generate a new one.");
        }

        if (stop.OTPCode != request.OTPCode)
        {
            return BadRequest("Invalid OTP code.");
        }

        stop.OTPVerified = true;
        stop.OTPVerifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Age verification successful", Verified = true });
    }

    [HttpPatch("stops/{stopId}/update-safe-place")]
    public async Task<IActionResult> UpdateSafePlace(int stopId, [FromBody] UpdateSafePlaceRequest request)
    {
        var stop = await _context.DeliveryStops.FindAsync(stopId);
        if (stop == null)
        {
            return NotFound();
        }

        stop.SafePlace = request.SafePlace;
        stop.DoorAccessCode = request.DoorAccessCode;
        stop.PostBoxCode = request.PostBoxCode;
        stop.BuildingAccessInstructions = request.BuildingAccessInstructions;
        
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/reorder-stops")]
    public async Task<IActionResult> ReorderStops(int id, [FromBody] ReorderStopsRequest request)
    {
        var route = await _context.DeliveryRoutes
            .Include(r => r.Stops)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (route == null)
        {
            return NotFound();
        }

        // Validate that all stop IDs are valid
        var stopIds = route.Stops.Select(s => s.Id).ToList();
        if (!request.StopIds.All(id => stopIds.Contains(id)))
        {
            return BadRequest("Invalid stop IDs provided");
        }

        // Update sequence numbers based on new order
        for (int i = 0; i < request.StopIds.Count; i++)
        {
            var stop = route.Stops.FirstOrDefault(s => s.Id == request.StopIds[i]);
            if (stop != null)
            {
                stop.SequenceNumber = i + 1;
            }
        }

        route.LastModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Stops reordered successfully" });
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
