using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockItemImagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StockItemImagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("stockitem/{stockItemId}")]
    public async Task<ActionResult<IEnumerable<StockItemImage>>> GetStockItemImages(int stockItemId)
    {
        var images = await _context.StockItemImages
            .Where(i => i.StockItemId == stockItemId)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync();

        return Ok(images);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockItemImage>> GetStockItemImage(int id)
    {
        var image = await _context.StockItemImages.FindAsync(id);

        if (image == null)
        {
            return NotFound();
        }

        return image;
    }

    [HttpPost]
    public async Task<ActionResult<StockItemImage>> PostStockItemImage(CreateStockItemImageDto dto)
    {
        // Check if setting as primary, if so, unset other primary images
        if (dto.IsPrimaryImage)
        {
            var existingPrimary = await _context.StockItemImages
                .Where(i => i.StockItemId == dto.StockItemId && i.IsPrimaryImage)
                .ToListAsync();
            
            foreach (var img in existingPrimary)
            {
                img.IsPrimaryImage = false;
            }
        }

        var image = new StockItemImage
        {
            StockItemId = dto.StockItemId,
            ImageUrl = dto.ImageUrl,
            ImageData = dto.ImageData,
            Caption = dto.Caption,
            DisplayOrder = dto.DisplayOrder,
            IsPrimaryImage = dto.IsPrimaryImage,
            CreatedDate = DateTime.UtcNow
        };

        _context.StockItemImages.Add(image);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStockItemImage), new { id = image.Id }, image);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutStockItemImage(int id, UpdateStockItemImageDto dto)
    {
        var image = await _context.StockItemImages.FindAsync(id);
        if (image == null)
        {
            return NotFound();
        }

        // Check if setting as primary, if so, unset other primary images
        if (dto.IsPrimaryImage && !image.IsPrimaryImage)
        {
            var existingPrimary = await _context.StockItemImages
                .Where(i => i.StockItemId == image.StockItemId && i.IsPrimaryImage && i.Id != id)
                .ToListAsync();
            
            foreach (var img in existingPrimary)
            {
                img.IsPrimaryImage = false;
            }
        }

        image.ImageUrl = dto.ImageUrl;
        image.ImageData = dto.ImageData;
        image.Caption = dto.Caption;
        image.DisplayOrder = dto.DisplayOrder;
        image.IsPrimaryImage = dto.IsPrimaryImage;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStockItemImage(int id)
    {
        var image = await _context.StockItemImages.FindAsync(id);
        if (image == null)
        {
            return NotFound();
        }

        _context.StockItemImages.Remove(image);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateStockItemImageDto
{
    public int StockItemId { get; set; }
    public required string ImageUrl { get; set; }
    public string? ImageData { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimaryImage { get; set; } = false;
}

public class UpdateStockItemImageDto
{
    public required string ImageUrl { get; set; }
    public string? ImageData { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimaryImage { get; set; }
}
