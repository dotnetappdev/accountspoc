using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PermissionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Permission>>> GetPermissions()
    {
        return await _context.Permissions.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Permission>> GetPermission(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);

        if (permission == null)
        {
            return NotFound();
        }

        return permission;
    }

    [HttpPost]
    public async Task<ActionResult<Permission>> PostPermission(CreatePermissionDto dto)
    {
        var permission = new Permission
        {
            Name = dto.Name,
            Description = dto.Description,
            Resource = dto.Resource,
            Action = dto.Action,
            CreatedDate = DateTime.UtcNow
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permission);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPermission(int id, UpdatePermissionDto dto)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null)
        {
            return NotFound();
        }

        permission.Name = dto.Name;
        permission.Description = dto.Description;
        permission.Resource = dto.Resource;
        permission.Action = dto.Action;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePermission(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null)
        {
            return NotFound();
        }

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreatePermissionDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Resource { get; set; }
    public required string Action { get; set; }
}

public class UpdatePermissionDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Resource { get; set; }
    public required string Action { get; set; }
}
