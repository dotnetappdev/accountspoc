using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DriversController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers([FromQuery] bool? isActive = null)
    {
        var query = _context.Drivers.AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(d => d.IsActive == isActive.Value);
        }

        return await query.OrderBy(d => d.LastName).ThenBy(d => d.FirstName).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Driver>> GetDriver(int id)
    {
        var driver = await _context.Drivers
            .Include(d => d.DeliveryRoutes)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (driver == null)
        {
            return NotFound();
        }

        return driver;
    }

    [HttpGet("by-code/{code}")]
    public async Task<ActionResult<Driver>> GetDriverByCode(string code)
    {
        var driver = await _context.Drivers
            .FirstOrDefaultAsync(d => d.DriverCode == code);

        if (driver == null)
        {
            return NotFound();
        }

        return driver;
    }

    [HttpPost]
    public async Task<ActionResult<Driver>> PostDriver(Driver driver)
    {
        driver.CreatedDate = DateTime.UtcNow;
        _context.Drivers.Add(driver);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (await _context.Drivers.AnyAsync(d => d.DriverCode == driver.DriverCode && d.TenantId == driver.TenantId))
            {
                return Conflict($"Driver with code '{driver.DriverCode}' already exists.");
            }
            throw;
        }

        return CreatedAtAction(nameof(GetDriver), new { id = driver.Id }, driver);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutDriver(int id, Driver driver)
    {
        if (id != driver.Id)
        {
            return BadRequest();
        }

        driver.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(driver).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Drivers.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateDriver(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null)
        {
            return NotFound();
        }

        driver.IsActive = false;
        driver.EmploymentEndDate = DateTime.UtcNow;
        driver.LastModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateDriver(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null)
        {
            return NotFound();
        }

        driver.IsActive = true;
        driver.EmploymentEndDate = null; // Clear end date when reactivating
        driver.LastModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDriver(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null)
        {
            return NotFound();
        }

        // Check if driver has any delivery routes
        var hasRoutes = await _context.DeliveryRoutes.AnyAsync(r => r.DriverId == id);
        if (hasRoutes)
        {
            return BadRequest("Cannot delete driver with associated delivery routes. Deactivate instead.");
        }

        _context.Drivers.Remove(driver);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/routes")]
    public async Task<ActionResult<IEnumerable<DeliveryRoute>>> GetDriverRoutes(int id, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
    {
        var query = _context.DeliveryRoutes
            .Where(r => r.DriverId == id)
            .Include(r => r.Stops)
            .AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(r => r.RouteDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(r => r.RouteDate <= toDate.Value);
        }

        return await query.OrderByDescending(r => r.RouteDate).ToListAsync();
    }
}
