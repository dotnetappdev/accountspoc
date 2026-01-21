using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExchangeRatesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private const string ECB_API_URL = "https://api.frankfurter.app";

    public ExchangeRatesController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    // GET: api/ExchangeRates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetExchangeRates([FromQuery] int tenantId = 1)
    {
        return await _context.ExchangeRates
            .Where(e => e.TenantId == tenantId)
            .OrderByDescending(e => e.RateDate)
            .ToListAsync();
    }

    // GET: api/ExchangeRates/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ExchangeRate>> GetExchangeRate(int id, [FromQuery] int tenantId = 1)
    {
        var exchangeRate = await _context.ExchangeRates
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);

        if (exchangeRate == null)
        {
            return NotFound();
        }

        return exchangeRate;
    }

    // GET: api/ExchangeRates/rate/{from}/{to}
    [HttpGet("rate/{from}/{to}")]
    public async Task<ActionResult<decimal>> GetRate(string from, string to, [FromQuery] int tenantId = 1)
    {
        // Try to get the latest rate from database
        var latestRate = await _context.ExchangeRates
            .Where(e => e.TenantId == tenantId && 
                       e.FromCurrency == from && 
                       e.ToCurrency == to && 
                       e.IsActive)
            .OrderByDescending(e => e.RateDate)
            .FirstOrDefaultAsync();

        // If rate is recent (less than 24 hours old), use it
        if (latestRate != null && (DateTime.UtcNow - latestRate.RateDate).TotalHours < 24)
        {
            return Ok(latestRate.Rate);
        }

        // Otherwise fetch from API
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"{ECB_API_URL}/latest?from={from}&to={to}");
            var data = JsonSerializer.Deserialize<FrankfurterResponse>(response);

            if (data?.Rates != null && data.Rates.ContainsKey(to))
            {
                return Ok(data.Rates[to]);
            }

            return BadRequest("Unable to fetch exchange rate");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching rate: {ex.Message}");
        }
    }

    // GET: api/ExchangeRates/convert
    [HttpGet("convert")]
    public async Task<ActionResult<decimal>> ConvertCurrency(
        [FromQuery] string from, 
        [FromQuery] string to, 
        [FromQuery] decimal amount,
        [FromQuery] int tenantId = 1)
    {
        if (from == to)
        {
            return Ok(amount);
        }

        var rateResult = await GetRate(from, to, tenantId);
        if (rateResult.Result is OkObjectResult okResult && okResult.Value is decimal rate)
        {
            return Ok(amount * rate);
        }

        return BadRequest("Unable to convert currency");
    }

    // POST: api/ExchangeRates/update-from-ecb
    [HttpPost("update-from-ecb")]
    public async Task<ActionResult<int>> UpdateFromECB([FromQuery] int tenantId = 1, [FromQuery] string baseCurrency = "USD")
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"{ECB_API_URL}/latest?from={baseCurrency}");
            var data = JsonSerializer.Deserialize<FrankfurterResponse>(response);

            if (data?.Rates == null)
            {
                return BadRequest("No rates returned from API");
            }

            int updated = 0;
            var rateDate = data.Date ?? DateTime.UtcNow.Date;

            foreach (var rate in data.Rates)
            {
                var exchangeRate = new ExchangeRate
                {
                    TenantId = tenantId,
                    FromCurrency = baseCurrency,
                    ToCurrency = rate.Key,
                    Rate = rate.Value,
                    RateDate = rateDate,
                    LastUpdated = DateTime.UtcNow,
                    Source = "ECB (Frankfurter)",
                    IsActive = true
                };

                // Deactivate old rates
                var oldRates = await _context.ExchangeRates
                    .Where(e => e.TenantId == tenantId && 
                               e.FromCurrency == baseCurrency && 
                               e.ToCurrency == rate.Key && 
                               e.IsActive)
                    .ToListAsync();

                foreach (var old in oldRates)
                {
                    old.IsActive = false;
                }

                _context.ExchangeRates.Add(exchangeRate);
                updated++;
            }

            await _context.SaveChangesAsync();

            return Ok(new { updated, baseCurrency, rateDate, source = "ECB (Frankfurter)" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating rates: {ex.Message}");
        }
    }

    // POST: api/ExchangeRates
    [HttpPost]
    public async Task<ActionResult<ExchangeRate>> CreateExchangeRate(ExchangeRate exchangeRate)
    {
        exchangeRate.LastUpdated = DateTime.UtcNow;
        exchangeRate.Source = "Manual";

        _context.ExchangeRates.Add(exchangeRate);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExchangeRate), new { id = exchangeRate.Id, tenantId = exchangeRate.TenantId }, exchangeRate);
    }

    // PUT: api/ExchangeRates/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExchangeRate(int id, ExchangeRate exchangeRate)
    {
        if (id != exchangeRate.Id)
        {
            return BadRequest();
        }

        exchangeRate.LastUpdated = DateTime.UtcNow;

        _context.Entry(exchangeRate).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ExchangeRateExists(id, exchangeRate.TenantId))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/ExchangeRates/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExchangeRate(int id, [FromQuery] int tenantId = 1)
    {
        var exchangeRate = await _context.ExchangeRates
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);

        if (exchangeRate == null)
        {
            return NotFound();
        }

        _context.ExchangeRates.Remove(exchangeRate);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/ExchangeRates/supported-currencies
    [HttpGet("supported-currencies")]
    public async Task<ActionResult<IEnumerable<string>>> GetSupportedCurrencies()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"{ECB_API_URL}/currencies");
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(response);

            if (data != null && data.Keys.Any())
            {
                return Ok(data.Keys.ToList());
            }
            
            // Return common currencies if API returns empty
            return Ok(new List<string> { "USD", "EUR", "GBP", "JPY", "AUD", "CAD", "CHF", "CNY", "INR", "BRL", "ZAR", "NZD", "SGD", "HKD", "MXN" });
        }
        catch (Exception ex)
        {
            // Return common currencies if API fails
            return Ok(new List<string> { "USD", "EUR", "GBP", "JPY", "AUD", "CAD", "CHF", "CNY", "INR", "BRL", "ZAR", "NZD", "SGD", "HKD", "MXN" });
        }
    }

    private async Task<bool> ExchangeRateExists(int id, int tenantId)
    {
        return await _context.ExchangeRates.AnyAsync(e => e.Id == id && e.TenantId == tenantId);
    }

    // Helper class for deserializing Frankfurter API response
    private class FrankfurterResponse
    {
        public decimal Amount { get; set; }
        public string Base { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }
}
