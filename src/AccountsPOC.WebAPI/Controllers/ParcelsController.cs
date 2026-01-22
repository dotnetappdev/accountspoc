using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParcelsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ParcelsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Parcel>>> GetParcels()
    {
        return await _context.Parcels
            .Include(p => p.Container)
            .Include(p => p.DeliveryStop)
            .Include(p => p.ScannedByDriver)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Parcel>> GetParcel(int id)
    {
        var parcel = await _context.Parcels
            .Include(p => p.Container)
            .Include(p => p.DeliveryStop)
            .Include(p => p.ScannedByDriver)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (parcel == null)
        {
            return NotFound();
        }

        return parcel;
    }

    [HttpGet("by-barcode/{barcode}")]
    public async Task<ActionResult<Parcel>> GetParcelByBarcode(string barcode)
    {
        var parcel = await _context.Parcels
            .Include(p => p.Container)
            .Include(p => p.DeliveryStop)
            .Include(p => p.ScannedByDriver)
            .FirstOrDefaultAsync(p => p.ParcelBarcode == barcode);

        if (parcel == null)
        {
            return NotFound();
        }

        return parcel;
    }

    [HttpPost("scan-to-van")]
    public async Task<IActionResult> ScanToVan([FromBody] ScanToVanRequest request)
    {
        var parcel = await _context.Parcels
            .FirstOrDefaultAsync(p => p.ParcelBarcode == request.ParcelBarcode);

        if (parcel == null)
        {
            return NotFound("Parcel not found");
        }

        if (parcel.Status == "Scanned")
        {
            return BadRequest("Parcel already scanned");
        }

        parcel.Status = "Scanned";
        parcel.ScannedToVanAt = DateTime.UtcNow;
        parcel.ScannedByDriverId = request.DriverId;
        parcel.ContainerId = request.ContainerId;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Parcel scanned successfully", Parcel = parcel });
    }

    [HttpPost]
    public async Task<ActionResult<Parcel>> CreateParcel(Parcel parcel)
    {
        parcel.CreatedDate = DateTime.UtcNow;
        _context.Parcels.Add(parcel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetParcel), new { id = parcel.Id }, parcel);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParcel(int id)
    {
        var parcel = await _context.Parcels.FindAsync(id);
        if (parcel == null)
        {
            return NotFound();
        }

        _context.Parcels.Remove(parcel);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class ContainersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ContainersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Container>>> GetContainers()
    {
        return await _context.Containers
            .Include(c => c.Parcels)
            .Include(c => c.Driver)
            .Include(c => c.DeliveryRoute)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Container>> GetContainer(int id)
    {
        var container = await _context.Containers
            .Include(c => c.Parcels)
            .Include(c => c.Driver)
            .Include(c => c.DeliveryRoute)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (container == null)
        {
            return NotFound();
        }

        return container;
    }

    [HttpGet("by-route/{routeId}")]
    public async Task<ActionResult<IEnumerable<Container>>> GetContainersByRoute(int routeId)
    {
        return await _context.Containers
            .Include(c => c.Parcels)
            .Where(c => c.DeliveryRouteId == routeId)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Container>> CreateContainer(Container container)
    {
        container.CreatedDate = DateTime.UtcNow;
        _context.Containers.Add(container);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetContainer), new { id = container.Id }, container);
    }

    [HttpPatch("{id}/assign-to-route")]
    public async Task<IActionResult> AssignToRoute(int id, [FromBody] AssignToRouteRequest request)
    {
        var container = await _context.Containers.FindAsync(id);
        if (container == null)
        {
            return NotFound();
        }

        container.DeliveryRouteId = request.DeliveryRouteId;
        container.DriverId = request.DriverId;
        container.Status = "InUse";
        container.LoadedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContainer(int id)
    {
        var container = await _context.Containers.FindAsync(id);
        if (container == null)
        {
            return NotFound();
        }

        _context.Containers.Remove(container);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public record ScanToVanRequest(string ParcelBarcode, int DriverId, int? ContainerId);
public record AssignToRouteRequest(int DeliveryRouteId, int DriverId);
