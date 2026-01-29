using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RolesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetRoles()
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Description,
                r.IsSystemRole,
                r.CreatedDate,
                r.LastModifiedDate,
                Permissions = r.RolePermissions.Select(rp => new
                {
                    rp.Permission.Id,
                    rp.Permission.Name,
                    rp.Permission.Resource,
                    rp.Permission.Action
                })
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetRole(int id)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Description,
                r.IsSystemRole,
                r.CreatedDate,
                r.LastModifiedDate,
                Permissions = r.RolePermissions.Select(rp => new
                {
                    rp.Permission.Id,
                    rp.Permission.Name,
                    rp.Permission.Resource,
                    rp.Permission.Action
                })
            })
            .FirstOrDefaultAsync();

        if (role == null)
        {
            return NotFound();
        }

        return role;
    }

    [HttpPost]
    public async Task<ActionResult<Role>> PostRole(CreateRoleDto dto)
    {
        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description,
            IsSystemRole = false,
            CreatedDate = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Assign permissions if provided
        if (dto.PermissionIds != null && dto.PermissionIds.Any())
        {
            foreach (var permissionId in dto.PermissionIds)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                    AssignedDate = DateTime.UtcNow
                };
                _context.RolePermissions.Add(rolePermission);
            }
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutRole(int id, UpdateRoleDto dto)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        if (role.IsSystemRole)
        {
            return BadRequest("Cannot modify system roles");
        }

        role.Name = dto.Name;
        role.Description = dto.Description;
        role.LastModifiedDate = DateTime.UtcNow;

        // Update permissions if provided
        if (dto.PermissionIds != null)
        {
            // Remove existing permissions
            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == id)
                .ToListAsync();
            _context.RolePermissions.RemoveRange(existingPermissions);

            // Add new permissions
            foreach (var permissionId in dto.PermissionIds)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                    AssignedDate = DateTime.UtcNow
                };
                _context.RolePermissions.Add(rolePermission);
            }
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        if (role.IsSystemRole)
        {
            return BadRequest("Cannot delete system roles");
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateRoleDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<int>? PermissionIds { get; set; }
}

public class UpdateRoleDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<int>? PermissionIds { get; set; }
}
