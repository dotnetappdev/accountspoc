using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailTemplatesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private const int DefaultTenantId = 1;

    public EmailTemplatesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmailTemplate>>> GetEmailTemplates()
    {
        return await _context.EmailTemplates
            .Where(e => e.TenantId == DefaultTenantId)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmailTemplate>> GetEmailTemplate(int id)
    {
        var emailTemplate = await _context.EmailTemplates
            .Where(e => e.Id == id && e.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();

        if (emailTemplate == null)
        {
            return NotFound();
        }

        return emailTemplate;
    }

    [HttpGet("by-code/{templateCode}")]
    public async Task<ActionResult<EmailTemplate>> GetEmailTemplateByCode(string templateCode)
    {
        var emailTemplate = await _context.EmailTemplates
            .Where(e => e.TenantId == DefaultTenantId && e.TemplateCode == templateCode && e.IsActive)
            .FirstOrDefaultAsync();

        if (emailTemplate == null)
        {
            return NotFound();
        }

        return emailTemplate;
    }

    [HttpPost]
    public async Task<ActionResult<EmailTemplate>> PostEmailTemplate(EmailTemplate emailTemplate)
    {
        emailTemplate.TenantId = DefaultTenantId;
        emailTemplate.CreatedDate = DateTime.UtcNow;
        
        _context.EmailTemplates.Add(emailTemplate);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmailTemplate), new { id = emailTemplate.Id }, emailTemplate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmailTemplate(int id, EmailTemplate emailTemplate)
    {
        if (id != emailTemplate.Id)
        {
            return BadRequest();
        }

        var existingTemplate = await _context.EmailTemplates
            .Where(e => e.Id == id && e.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();

        if (existingTemplate == null)
        {
            return NotFound();
        }

        emailTemplate.TenantId = DefaultTenantId;
        emailTemplate.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(existingTemplate).CurrentValues.SetValues(emailTemplate);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmailTemplateExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmailTemplate(int id)
    {
        var emailTemplate = await _context.EmailTemplates
            .Where(e => e.Id == id && e.TenantId == DefaultTenantId)
            .FirstOrDefaultAsync();
            
        if (emailTemplate == null)
        {
            return NotFound();
        }

        _context.EmailTemplates.Remove(emailTemplate);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EmailTemplateExists(int id)
    {
        return _context.EmailTemplates.Any(e => e.Id == id && e.TenantId == DefaultTenantId);
    }
}
