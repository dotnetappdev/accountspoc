using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UsersController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Select(u => new
            {
                u.Id,
                u.TenantId,
                Username = u.UserName,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                u.IsActive,
                u.CreatedDate,
                u.LastModifiedDate,
                u.LastLoginDate,
                Roles = u.UserRoles.Select(ur => new { ur.Role.Id, ur.Role.Name })
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.TenantId,
                Username = u.UserName,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                u.IsActive,
                u.CreatedDate,
                u.LastModifiedDate,
                u.LastLoginDate,
                Roles = u.UserRoles.Select(ur => new { ur.Role.Id, ur.Role.Name })
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPost]
    public async Task<ActionResult<User>> PostUser(CreateUserDto dto)
    {
        // Validate tenant exists
        if (!await _context.Tenants.AnyAsync(t => t.Id == dto.TenantId))
        {
            return BadRequest("Invalid TenantId. Tenant does not exist.");
        }

        // Validate username uniqueness
        if (await _userManager.FindByNameAsync(dto.Username) != null)
        {
            return BadRequest("Username already exists.");
        }

        // Validate email uniqueness
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
        {
            return BadRequest("Email already exists.");
        }

        var user = new User
        {
            TenantId = dto.TenantId,
            UserName = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { error = "Failed to create user", errors = result.Errors.Select(e => e.Description) });
        }

        // Assign roles if provided
        if (dto.RoleIds != null && dto.RoleIds.Any())
        {
            // Get role names from IDs
            var roles = await _context.Roles.Where(r => dto.RoleIds.Contains(r.Id)).ToListAsync();
            foreach (var role in roles)
            {
                await _userManager.AddToRoleAsync(user, role.Name!);
            }

            // Update UserRoles with AssignedDate
            var userRoles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync();
            foreach (var userRole in userRoles)
            {
                userRole.AssignedDate = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        user.Email = dto.Email;
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;
        user.IsActive = dto.IsActive;
        user.LastModifiedDate = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return BadRequest(new { error = "Failed to update user", errors = updateResult.Errors.Select(e => e.Description) });
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);
            if (!passwordResult.Succeeded)
            {
                return BadRequest(new { error = "Failed to update password", errors = passwordResult.Errors.Select(e => e.Description) });
            }
        }

        // Update roles if provided
        if (dto.RoleIds != null)
        {
            // Get role names from IDs
            var roles = await _context.Roles.Where(r => dto.RoleIds.Contains(r.Id)).ToListAsync();
            var roleNames = roles.Select(r => r.Name!).ToList();
            
            // Remove existing roles
            var existingRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, existingRoles);
            
            // Add new roles
            await _userManager.AddToRolesAsync(user, roleNames);

            // Update UserRoles with AssignedDate
            var userRoles = await _context.UserRoles.Where(ur => ur.UserId == id).ToListAsync();
            foreach (var userRole in userRoles)
            {
                userRole.AssignedDate = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new { error = "Failed to delete user", errors = result.Errors.Select(e => e.Description) });
        }

        return NoContent();
    }
}

public class CreateUserDto
{
    public int TenantId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public List<int>? RoleIds { get; set; }
}

public class UpdateUserDto
{
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public string? Password { get; set; }
    public List<int>? RoleIds { get; set; }
}
