using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ApplicationDbContext _context;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        IJwtTokenService jwtTokenService,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        // Validate tenant exists
        if (!await _context.Tenants.AnyAsync(t => t.Id == dto.TenantId))
        {
            return BadRequest(new { error = "Invalid TenantId. Tenant does not exist." });
        }

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return BadRequest(new { error = "Email already exists." });
        }

        existingUser = await _userManager.FindByNameAsync(dto.Username);
        if (existingUser != null)
        {
            return BadRequest(new { error = "Username already exists." });
        }

        var user = new User
        {
            UserName = dto.Username,
            Email = dto.Email,
            TenantId = dto.TenantId,
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

        // Assign default User role
        if (await _roleManager.RoleExistsAsync("User"))
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        // Assign additional roles if provided
        if (dto.RoleNames != null && dto.RoleNames.Any())
        {
            foreach (var roleName in dto.RoleNames)
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }
        }

        // Update UserRole with AssignedDate
        var userRoles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync();
        foreach (var userRole in userRoles)
        {
            userRole.AssignedDate = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();

        var token = await _jwtTokenService.GenerateTokenAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Username = user.UserName!,
            UserId = user.Id,
            TenantId = user.TenantId
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            user = await _userManager.FindByNameAsync(dto.Email); // Allow login with username
            if (user == null)
            {
                return Unauthorized(new { error = "Invalid credentials" });
            }
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { error = "User account is inactive" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { error = "Invalid credentials" });
        }

        // Update last login date
        user.LastLoginDate = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var token = await _jwtTokenService.GenerateTokenAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Username = user.UserName!,
            UserId = user.Id,
            TenantId = user.TenantId
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserInfoDto>> GetCurrentUser()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = await _userManager.FindByIdAsync(userId.ToString());
        
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new UserInfoDto
        {
            UserId = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            TenantId = user.TenantId,
            IsActive = user.IsActive,
            Roles = roles.ToList()
        });
    }
}

public class RegisterDto
{
    public int TenantId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string>? RoleNames { get; set; }
}

public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class AuthResponseDto
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public int UserId { get; set; }
    public int TenantId { get; set; }
}

public class UserInfoDto
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public int TenantId { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}
