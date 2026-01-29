using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.Domain.Entities;

namespace AccountsPOC.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomFormsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomFormsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/CustomForms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomForm>>> GetCustomForms()
    {
        return await _context.CustomForms
            .Where(f => f.IsActive)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();
    }

    // GET: api/CustomForms/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomForm>> GetCustomForm(int id)
    {
        var customForm = await _context.CustomForms
            .Include(f => f.Submissions)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (customForm == null)
        {
            return NotFound();
        }

        return customForm;
    }

    // POST: api/CustomForms
    [HttpPost]
    public async Task<ActionResult<CustomForm>> PostCustomForm(CustomForm customForm)
    {
        customForm.CreatedDate = DateTime.UtcNow;
        customForm.TenantId = 1; // Default tenant for POC
        
        _context.CustomForms.Add(customForm);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCustomForm), new { id = customForm.Id }, customForm);
    }

    // PUT: api/CustomForms/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomForm(int id, CustomForm customForm)
    {
        if (id != customForm.Id)
        {
            return BadRequest();
        }

        customForm.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(customForm).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CustomFormExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/CustomForms/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomForm(int id)
    {
        var customForm = await _context.CustomForms.FindAsync(id);
        if (customForm == null)
        {
            return NotFound();
        }

        // Soft delete
        customForm.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/CustomForms/5/submissions
    [HttpGet("{id}/submissions")]
    public async Task<ActionResult<IEnumerable<FormSubmission>>> GetFormSubmissions(int id)
    {
        var submissions = await _context.FormSubmissions
            .Where(s => s.CustomFormId == id)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();

        return submissions;
    }

    private bool CustomFormExists(int id)
    {
        return _context.CustomForms.Any(e => e.Id == id);
    }
}
