using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandingAssetsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private const int CurrentTenantId = 1; // TODO: Get from authenticated user

    public BrandingAssetsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandingAsset>>> GetBrandingAssets()
    {
        return await _context.BrandingAssets
            .Where(ba => ba.TenantId == CurrentTenantId)
            .OrderBy(ba => ba.AssetType)
            .ThenBy(ba => ba.DisplayOrder)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BrandingAsset>> GetBrandingAsset(int id)
    {
        var brandingAsset = await _context.BrandingAssets
            .Where(ba => ba.Id == id && ba.TenantId == CurrentTenantId)
            .FirstOrDefaultAsync();

        if (brandingAsset == null)
        {
            return NotFound();
        }

        return brandingAsset;
    }

    [HttpGet("by-type/{assetType}")]
    public async Task<ActionResult<IEnumerable<BrandingAsset>>> GetByAssetType(string assetType)
    {
        return await _context.BrandingAssets
            .Where(ba => ba.TenantId == CurrentTenantId && ba.AssetType == assetType)
            .OrderBy(ba => ba.DisplayOrder)
            .ToListAsync();
    }

    [HttpGet("by-type/{assetType}/active")]
    public async Task<ActionResult<BrandingAsset>> GetActiveByType(string assetType)
    {
        var asset = await _context.BrandingAssets
            .Where(ba => ba.TenantId == CurrentTenantId && ba.AssetType == assetType && ba.IsActive)
            .OrderBy(ba => ba.DisplayOrder)
            .FirstOrDefaultAsync();

        if (asset == null)
        {
            return NotFound();
        }

        return asset;
    }

    [HttpPost]
    public async Task<ActionResult<BrandingAsset>> CreateBrandingAsset(BrandingAsset brandingAsset)
    {
        brandingAsset.TenantId = CurrentTenantId;
        brandingAsset.CreatedDate = DateTime.UtcNow;
        
        _context.BrandingAssets.Add(brandingAsset);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBrandingAsset), new { id = brandingAsset.Id }, brandingAsset);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<BrandingAsset>> UploadBrandingAsset([FromBody] BrandingAssetUploadRequest request)
    {
        // Validate base64 image
        if (string.IsNullOrEmpty(request.ImageDataBase64))
        {
            return BadRequest("Image data is required");
        }

        // Optionally validate max file size
        var estimatedSizeBytes = (request.ImageDataBase64.Length * 3) / 4;
        if (estimatedSizeBytes > 5 * 1024 * 1024) // 5MB limit
        {
            return BadRequest("File size exceeds 5MB limit");
        }

        var brandingAsset = new BrandingAsset
        {
            TenantId = CurrentTenantId,
            AssetType = request.AssetType,
            AssetName = request.AssetName,
            FileName = request.FileName,
            MimeType = request.MimeType,
            ImageData = request.ImageDataBase64,
            AltText = request.AltText,
            Width = request.Width,
            Height = request.Height,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
            Description = request.Description,
            UsageContext = request.UsageContext,
            FileSizeBytes = estimatedSizeBytes,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "System" // TODO: Get from authenticated user
        };

        // If setting as active, deactivate other assets of same type
        if (request.IsActive)
        {
            var existingActive = await _context.BrandingAssets
                .Where(ba => ba.TenantId == CurrentTenantId && 
                            ba.AssetType == request.AssetType && 
                            ba.IsActive)
                .ToListAsync();

            foreach (var asset in existingActive)
            {
                asset.IsActive = false;
                asset.LastModifiedDate = DateTime.UtcNow;
            }
        }

        _context.BrandingAssets.Add(brandingAsset);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBrandingAsset), new { id = brandingAsset.Id }, brandingAsset);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBrandingAsset(int id, BrandingAsset brandingAsset)
    {
        if (id != brandingAsset.Id)
        {
            return BadRequest();
        }

        var existingAsset = await _context.BrandingAssets
            .Where(ba => ba.Id == id && ba.TenantId == CurrentTenantId)
            .FirstOrDefaultAsync();

        if (existingAsset == null)
        {
            return NotFound();
        }

        // If setting as active, deactivate other assets of same type
        if (brandingAsset.IsActive && !existingAsset.IsActive)
        {
            var otherActive = await _context.BrandingAssets
                .Where(ba => ba.TenantId == CurrentTenantId && 
                            ba.AssetType == brandingAsset.AssetType && 
                            ba.IsActive && 
                            ba.Id != id)
                .ToListAsync();

            foreach (var asset in otherActive)
            {
                asset.IsActive = false;
                asset.LastModifiedDate = DateTime.UtcNow;
            }
        }

        existingAsset.AssetType = brandingAsset.AssetType;
        existingAsset.AssetName = brandingAsset.AssetName;
        existingAsset.FileName = brandingAsset.FileName;
        existingAsset.MimeType = brandingAsset.MimeType;
        existingAsset.ImageData = brandingAsset.ImageData;
        existingAsset.AltText = brandingAsset.AltText;
        existingAsset.Width = brandingAsset.Width;
        existingAsset.Height = brandingAsset.Height;
        existingAsset.IsActive = brandingAsset.IsActive;
        existingAsset.DisplayOrder = brandingAsset.DisplayOrder;
        existingAsset.Description = brandingAsset.Description;
        existingAsset.UsageContext = brandingAsset.UsageContext;
        existingAsset.FileSizeBytes = brandingAsset.FileSizeBytes;
        existingAsset.LastModifiedDate = DateTime.UtcNow;
        existingAsset.LastModifiedBy = "System"; // TODO: Get from authenticated user

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await BrandingAssetExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBrandingAsset(int id)
    {
        var brandingAsset = await _context.BrandingAssets
            .Where(ba => ba.Id == id && ba.TenantId == CurrentTenantId)
            .FirstOrDefaultAsync();

        if (brandingAsset == null)
        {
            return NotFound();
        }

        _context.BrandingAssets.Remove(brandingAsset);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/set-active")]
    public async Task<IActionResult> SetActive(int id)
    {
        var brandingAsset = await _context.BrandingAssets
            .Where(ba => ba.Id == id && ba.TenantId == CurrentTenantId)
            .FirstOrDefaultAsync();

        if (brandingAsset == null)
        {
            return NotFound();
        }

        // Deactivate other assets of same type
        var otherActive = await _context.BrandingAssets
            .Where(ba => ba.TenantId == CurrentTenantId && 
                        ba.AssetType == brandingAsset.AssetType && 
                        ba.IsActive && 
                        ba.Id != id)
            .ToListAsync();

        foreach (var asset in otherActive)
        {
            asset.IsActive = false;
            asset.LastModifiedDate = DateTime.UtcNow;
        }

        brandingAsset.IsActive = true;
        brandingAsset.LastModifiedDate = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> BrandingAssetExists(int id)
    {
        return await _context.BrandingAssets
            .AnyAsync(e => e.Id == id && e.TenantId == CurrentTenantId);
    }
}

// Request model for uploading branding assets
public class BrandingAssetUploadRequest
{
    public required string AssetType { get; set; }
    public required string AssetName { get; set; }
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public required string ImageDataBase64 { get; set; }
    public string? AltText { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public string? Description { get; set; }
    public string? UsageContext { get; set; }
}
