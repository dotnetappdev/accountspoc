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
            .Include(b => b.Images)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BillOfMaterial>> GetBillOfMaterial(int id)
    {
        var billOfMaterial = await _context.BillOfMaterials
            .Include(b => b.Product)
            .Include(b => b.Components)
            .ThenInclude(c => c.Product)
            .Include(b => b.Images)
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
        
        int lineNumber = 1;
        foreach (var component in billOfMaterial.Components)
        {
            component.LineNumber = lineNumber++;
            component.TotalCost = component.Quantity * component.UnitCost;
        }
        
        billOfMaterial.MaterialCost = billOfMaterial.Components.Sum(c => c.TotalCost);
        billOfMaterial.EstimatedCost = billOfMaterial.MaterialCost.GetValueOrDefault() 
            + billOfMaterial.LabourCost.GetValueOrDefault() 
            + billOfMaterial.OverheadCost.GetValueOrDefault();
        billOfMaterial.TotalCost = billOfMaterial.EstimatedCost;
        
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
        
        var existingBOM = await _context.BillOfMaterials
            .Include(b => b.Components)
            .Include(b => b.Images)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (existingBOM == null)
        {
            return NotFound();
        }

        _context.Entry(existingBOM).CurrentValues.SetValues(billOfMaterial);

        var existingComponents = existingBOM.Components.ToList();
        foreach (var existingComponent in existingComponents)
        {
            if (!billOfMaterial.Components.Any(c => c.Id == existingComponent.Id))
            {
                _context.BOMComponents.Remove(existingComponent);
            }
        }

        int lineNumber = 1;
        foreach (var component in billOfMaterial.Components)
        {
            component.LineNumber = lineNumber++;
            component.TotalCost = component.Quantity * component.UnitCost;
            
            var existingComponent = existingComponents.FirstOrDefault(c => c.Id == component.Id);
            if (existingComponent != null)
            {
                _context.Entry(existingComponent).CurrentValues.SetValues(component);
            }
            else
            {
                component.BillOfMaterialId = id;
                existingBOM.Components.Add(component);
            }
        }

        var existingImages = existingBOM.Images.ToList();
        foreach (var existingImage in existingImages)
        {
            if (!billOfMaterial.Images.Any(i => i.Id == existingImage.Id))
            {
                _context.BOMImages.Remove(existingImage);
            }
        }

        foreach (var image in billOfMaterial.Images)
        {
            var existingImage = existingImages.FirstOrDefault(i => i.Id == image.Id);
            if (existingImage != null)
            {
                _context.Entry(existingImage).CurrentValues.SetValues(image);
            }
            else
            {
                image.BillOfMaterialId = id;
                image.CreatedDate = DateTime.UtcNow;
                existingBOM.Images.Add(image);
            }
        }

        existingBOM.MaterialCost = existingBOM.Components.Sum(c => c.TotalCost);
        existingBOM.EstimatedCost = existingBOM.MaterialCost.GetValueOrDefault() 
            + existingBOM.LabourCost.GetValueOrDefault() 
            + existingBOM.OverheadCost.GetValueOrDefault();
        existingBOM.TotalCost = existingBOM.EstimatedCost;

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
