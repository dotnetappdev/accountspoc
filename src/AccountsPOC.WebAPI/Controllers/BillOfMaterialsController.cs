using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillOfMaterialsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BillOfMaterialsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BillOfMaterial>>> GetBillOfMaterials()
    {
        return await _context.BillOfMaterials
            .Include(b => b.Product)
            .Include(b => b.Components)
            .ThenInclude(c => c.Product)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BillOfMaterial>> GetBillOfMaterial(int id)
    {
        var billOfMaterial = await _context.BillOfMaterials
            .Include(b => b.Product)
            .Include(b => b.Components)
            .ThenInclude(c => c.Product)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (billOfMaterial == null)
        {
            return NotFound();
        }

        return billOfMaterial;
    }

    [HttpPost]
    public async Task<ActionResult<BillOfMaterial>> PostBillOfMaterial(BillOfMaterial billOfMaterial)
    {
        billOfMaterial.CreatedDate = DateTime.UtcNow;
        
        foreach (var component in billOfMaterial.Components)
        {
            component.TotalCost = component.Quantity * component.UnitCost;
        }
        
        billOfMaterial.EstimatedCost = billOfMaterial.Components.Sum(c => c.TotalCost);
        
        _context.BillOfMaterials.Add(billOfMaterial);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBillOfMaterial), new { id = billOfMaterial.Id }, billOfMaterial);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutBillOfMaterial(int id, BillOfMaterial billOfMaterial)
    {
        if (id != billOfMaterial.Id)
        {
            return BadRequest();
        }

        billOfMaterial.LastModifiedDate = DateTime.UtcNow;
        
        foreach (var component in billOfMaterial.Components)
        {
            component.TotalCost = component.Quantity * component.UnitCost;
        }
        
        billOfMaterial.EstimatedCost = billOfMaterial.Components.Sum(c => c.TotalCost);
        
        _context.Entry(billOfMaterial).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BillOfMaterialExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBillOfMaterial(int id)
    {
        var billOfMaterial = await _context.BillOfMaterials.FindAsync(id);
        if (billOfMaterial == null)
        {
            return NotFound();
        }

        _context.BillOfMaterials.Remove(billOfMaterial);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BillOfMaterialExists(int id)
    {
        return _context.BillOfMaterials.Any(e => e.Id == id);
    }
}
