using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.Domain.Entities;

namespace AccountsPOC.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FormSubmissionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FormSubmissionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/FormSubmissions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FormSubmission>>> GetFormSubmissions()
    {
        return await _context.FormSubmissions
            .Include(s => s.CustomForm)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }

    // GET: api/FormSubmissions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<FormSubmission>> GetFormSubmission(int id)
    {
        var formSubmission = await _context.FormSubmissions
            .Include(s => s.CustomForm)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (formSubmission == null)
        {
            return NotFound();
        }

        return formSubmission;
    }

    // POST: api/FormSubmissions
    [HttpPost]
    public async Task<ActionResult<FormSubmission>> PostFormSubmission(FormSubmission formSubmission)
    {
        formSubmission.SubmittedDate = DateTime.UtcNow;
        formSubmission.TenantId = 1; // Default tenant for POC
        formSubmission.SubmitterIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        _context.FormSubmissions.Add(formSubmission);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFormSubmission), new { id = formSubmission.Id }, formSubmission);
    }

    // DELETE: api/FormSubmissions/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFormSubmission(int id)
    {
        var formSubmission = await _context.FormSubmissions.FindAsync(id);
        if (formSubmission == null)
        {
            return NotFound();
        }

        _context.FormSubmissions.Remove(formSubmission);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
