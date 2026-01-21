using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly int _tenantId = 1; // For POC - in production, get from auth context

    public PurchaseOrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrders()
    {
        return await _context.PurchaseOrders
            .Where(po => po.TenantId == _tenantId)
            .Include(po => po.Supplier)
            .Include(po => po.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrder(int id)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(po => po.Id == id && po.TenantId == _tenantId);

        if (purchaseOrder == null)
        {
            return NotFound();
        }

        return purchaseOrder;
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseOrder>> CreatePurchaseOrder(PurchaseOrder purchaseOrder)
    {
        purchaseOrder.TenantId = _tenantId;
        purchaseOrder.CreatedDate = DateTime.UtcNow;
        purchaseOrder.OrderDate = DateTime.UtcNow;

        // Generate order number if not provided
        if (string.IsNullOrEmpty(purchaseOrder.OrderNumber))
        {
            purchaseOrder.OrderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }

        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPurchaseOrder), new { id = purchaseOrder.Id }, purchaseOrder);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePurchaseOrder(int id, PurchaseOrder purchaseOrder)
    {
        if (id != purchaseOrder.Id)
        {
            return BadRequest();
        }

        var existingPO = await _context.PurchaseOrders
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.Id == id && po.TenantId == _tenantId);

        if (existingPO == null)
        {
            return NotFound();
        }

        existingPO.SupplierId = purchaseOrder.SupplierId;
        existingPO.ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate;
        existingPO.ActualDeliveryDate = purchaseOrder.ActualDeliveryDate;
        existingPO.Status = purchaseOrder.Status;
        existingPO.SubTotal = purchaseOrder.SubTotal;
        existingPO.TaxAmount = purchaseOrder.TaxAmount;
        existingPO.TotalAmount = purchaseOrder.TotalAmount;
        existingPO.Notes = purchaseOrder.Notes;
        existingPO.DeliveryAddress = purchaseOrder.DeliveryAddress;
        existingPO.LastModifiedDate = DateTime.UtcNow;

        // Update items
        _context.PurchaseOrderItems.RemoveRange(existingPO.Items);
        existingPO.Items = purchaseOrder.Items;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.PurchaseOrders.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePurchaseOrder(int id)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .FirstOrDefaultAsync(po => po.Id == id && po.TenantId == _tenantId);

        if (purchaseOrder == null)
        {
            return NotFound();
        }

        _context.PurchaseOrders.Remove(purchaseOrder);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Generate reorder list based on stock levels
    [HttpGet("reorder-list")]
    public async Task<ActionResult<object>> GetReorderList()
    {
        var products = await _context.Products
            .Where(p => p.TenantId == _tenantId && p.StockLevel <= p.ReorderLevel && p.IsActive)
            .Include(p => p.PreferredSupplier)
            .ToListAsync();

        var reorderList = products.Select(p => new
        {
            p.Id,
            p.ProductCode,
            p.ProductName,
            p.StockLevel,
            p.ReorderLevel,
            QuantityNeeded = p.ReorderLevel * 2 - p.StockLevel,
            SupplierId = p.PreferredSupplierId,
            SupplierName = p.PreferredSupplier?.SupplierName,
            SupplierEmail = p.PreferredSupplier?.Email,
            UnitCost = p.SupplierUnitCost ?? p.UnitPrice,
            EstimatedTotal = (p.ReorderLevel * 2 - p.StockLevel) * (p.SupplierUnitCost ?? p.UnitPrice)
        }).ToList();

        return Ok(reorderList);
    }

    // Export reorder list as CSV
    [HttpGet("reorder-list/export-csv")]
    public async Task<IActionResult> ExportReorderListCsv([FromQuery] int? supplierId = null)
    {
        var query = _context.Products
            .Where(p => p.TenantId == _tenantId && p.StockLevel <= p.ReorderLevel && p.IsActive)
            .Include(p => p.PreferredSupplier)
            .AsQueryable();

        if (supplierId.HasValue)
        {
            query = query.Where(p => p.PreferredSupplierId == supplierId.Value);
        }

        var products = await query.ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("Product Code,Product Name,Current Stock,Reorder Level,Quantity Needed,Supplier,Supplier Email,Unit Cost,Estimated Total");

        foreach (var product in products)
        {
            var quantityNeeded = product.ReorderLevel * 2 - product.StockLevel;
            var unitCost = product.SupplierUnitCost ?? product.UnitPrice;
            var estimatedTotal = quantityNeeded * unitCost;

            csv.AppendLine($"\"{product.ProductCode}\",\"{product.ProductName}\",{product.StockLevel},{product.ReorderLevel},{quantityNeeded},\"{product.PreferredSupplier?.SupplierName ?? "N/A"}\",\"{product.PreferredSupplier?.Email ?? "N/A"}\",{unitCost:F2},{estimatedTotal:F2}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"Reorder_List_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    // Send reorder list to supplier via email
    [HttpPost("reorder-list/send-email")]
    public async Task<IActionResult> SendReorderListEmail([FromBody] SendReorderEmailRequest request)
    {
        try
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == request.SupplierId && s.TenantId == _tenantId);

            if (supplier == null)
            {
                return NotFound("Supplier not found");
            }

            var products = await _context.Products
                .Where(p => p.TenantId == _tenantId 
                    && p.StockLevel <= p.ReorderLevel 
                    && p.IsActive 
                    && p.PreferredSupplierId == request.SupplierId)
                .ToListAsync();

            if (products.Count == 0)
            {
                return BadRequest("No products need reordering from this supplier");
            }

            // Generate CSV
            var csv = new StringBuilder();
            csv.AppendLine("Product Code,Product Name,Current Stock,Reorder Level,Quantity Needed,Unit Cost");

            foreach (var product in products)
            {
                var quantityNeeded = product.ReorderLevel * 2 - product.StockLevel;
                var unitCost = product.SupplierUnitCost ?? product.UnitPrice;
                csv.AppendLine($"\"{product.ProductCode}\",\"{product.ProductName}\",{product.StockLevel},{product.ReorderLevel},{quantityNeeded},{unitCost:F2}");
            }

            // Create email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Company", "noreply@yourcompany.com"));
            message.To.Add(new MailboxAddress(supplier.SupplierName, supplier.Email));
            message.Subject = $"Stock Reorder Request - {DateTime.UtcNow:yyyy-MM-dd}";

            var builder = new BodyBuilder();
            builder.TextBody = request.Message ?? $"Dear {supplier.ContactName},\n\nPlease find attached our stock reorder requirements.\n\nBest regards";

            // Attach CSV
            builder.Attachments.Add($"Reorder_List_{DateTime.UtcNow:yyyyMMdd}.csv", Encoding.UTF8.GetBytes(csv.ToString()));

            message.Body = builder.ToMessageBody();

            // Send email (in production, configure SMTP settings from SystemSettings)
            using var client = new SmtpClient();
            // await client.ConnectAsync("smtp.yourserver.com", 587, false);
            // await client.AuthenticateAsync("username", "password");
            // await client.SendAsync(message);
            // await client.DisconnectAsync(true);

            // For POC, just return success
            return Ok(new { success = true, message = "Email would be sent in production environment", recipientEmail = supplier.Email });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class SendReorderEmailRequest
{
    public int SupplierId { get; set; }
    public string? Message { get; set; }
}
