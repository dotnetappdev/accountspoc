using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public QuotesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes()
    {
        return await _context.Quotes
            .Include(q => q.QuoteItems)
            .ThenInclude(i => i.Product)
            .Include(q => q.Customer)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Quote>> GetQuote(int id)
    {
        var quote = await _context.Quotes
            .Include(q => q.QuoteItems)
            .ThenInclude(i => i.Product)
            .Include(q => q.Customer)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quote == null)
        {
            return NotFound();
        }

        return quote;
    }

    [HttpPost]
    public async Task<ActionResult<Quote>> PostQuote(Quote quote)
    {
        quote.CreatedDate = DateTime.UtcNow;
        quote.QuoteDate = DateTime.UtcNow;
        
        // Calculate totals only if items exist
        if (quote.QuoteItems != null && quote.QuoteItems.Any())
        {
            foreach (var item in quote.QuoteItems)
            {
                item.LineTotal = (item.Quantity * item.UnitPrice) * (1 - item.DiscountPercent / 100) * (1 + item.TaxRate / 100);
            }
            
            quote.SubTotal = quote.QuoteItems.Sum(i => i.Quantity * i.UnitPrice);
            quote.DiscountAmount = quote.QuoteItems.Sum(i => i.Quantity * i.UnitPrice * i.DiscountPercent / 100);
            quote.TaxAmount = quote.QuoteItems.Sum(i => (i.Quantity * i.UnitPrice * (1 - i.DiscountPercent / 100)) * i.TaxRate / 100);
            quote.TotalAmount = quote.SubTotal - quote.DiscountAmount + quote.TaxAmount;
        }
        
        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutQuote(int id, Quote quote)
    {
        if (id != quote.Id)
        {
            return BadRequest();
        }

        quote.LastModifiedDate = DateTime.UtcNow;
        
        // Calculate totals only if items exist
        if (quote.QuoteItems != null && quote.QuoteItems.Any())
        {
            foreach (var item in quote.QuoteItems)
            {
                item.LineTotal = (item.Quantity * item.UnitPrice) * (1 - item.DiscountPercent / 100) * (1 + item.TaxRate / 100);
            }
            
            quote.SubTotal = quote.QuoteItems.Sum(i => i.Quantity * i.UnitPrice);
            quote.DiscountAmount = quote.QuoteItems.Sum(i => i.Quantity * i.UnitPrice * i.DiscountPercent / 100);
            quote.TaxAmount = quote.QuoteItems.Sum(i => (i.Quantity * i.UnitPrice * (1 - i.DiscountPercent / 100)) * i.TaxRate / 100);
            quote.TotalAmount = quote.SubTotal - quote.DiscountAmount + quote.TaxAmount;
        }
        
        _context.Entry(quote).State = EntityState.Modified;

        // Handle quote items - remove and re-add in transaction
        var existingItems = await _context.QuoteItems.Where(i => i.QuoteId == id).ToListAsync();
        _context.QuoteItems.RemoveRange(existingItems);
        
        if (quote.QuoteItems != null)
        {
            _context.QuoteItems.AddRange(quote.QuoteItems);
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuoteExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuote(int id)
    {
        var quote = await _context.Quotes.FindAsync(id);
        if (quote == null)
        {
            return NotFound();
        }

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/convert-to-order")]
    public async Task<ActionResult<SalesOrder>> ConvertToSalesOrder(int id)
    {
        var quote = await _context.Quotes
            .Include(q => q.QuoteItems)
            .ThenInclude(i => i.Product)
            .Include(q => q.Customer)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quote == null)
        {
            return NotFound();
        }

        if (quote.Status == "Converted")
        {
            return BadRequest("Quote has already been converted to a sales order.");
        }

        // Create sales order from quote
        var salesOrder = new SalesOrder
        {
            TenantId = quote.TenantId,
            OrderNumber = $"SO-{DateTime.UtcNow:yyyyMMddHHmmss}",
            OrderDate = DateTime.UtcNow,
            CustomerId = quote.CustomerId,
            CustomerName = quote.CustomerName,
            CustomerEmail = quote.CustomerEmail,
            CustomerPhone = quote.CustomerPhone,
            CustomerReference = quote.CustomerReference,
            SubTotal = quote.SubTotal,
            TaxAmount = quote.TaxAmount,
            DiscountAmount = quote.DiscountAmount,
            TotalAmount = quote.TotalAmount,
            CurrencyCode = quote.CurrencyCode,
            ExchangeRate = quote.ExchangeRate,
            Status = "Pending",
            CreatedDate = DateTime.UtcNow,
            CreatedBy = quote.CreatedBy,
            InternalNotes = $"Converted from Quote {quote.QuoteNumber}",
            CustomerNotes = quote.CustomerNotes
        };

        // Create sales order items from quote items
        foreach (var quoteItem in quote.QuoteItems)
        {
            salesOrder.SalesOrderItems.Add(new SalesOrderItem
            {
                ProductId = quoteItem.ProductId,
                Description = quoteItem.Description,
                Quantity = (int)quoteItem.Quantity,
                UnitPrice = quoteItem.UnitPrice,
                TotalPrice = quoteItem.LineTotal
            });
        }

        _context.SalesOrders.Add(salesOrder);
        await _context.SaveChangesAsync();

        // Update quote status with correct order ID after saving
        quote.Status = "Converted";
        quote.AcceptedDate = DateTime.UtcNow;
        quote.ConvertedToOrderId = salesOrder.Id;
        quote.LastModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSalesOrder", "SalesOrders", new { id = salesOrder.Id }, salesOrder);
    }

    private bool QuoteExists(int id)
    {
        return _context.Quotes.Any(e => e.Id == id);
    }
}
