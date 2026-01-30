using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WorkOrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkOrder>>> GetWorkOrders()
    {
        return await _context.WorkOrders
            .Include(w => w.Customer)
            .Include(w => w.AssignedTo)
            .Include(w => w.WorkOrderTasks)
            .Include(w => w.SiteVisitSignOffs)
            .Include(w => w.SalesOrder)
            .Include(w => w.Quote)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkOrder>> GetWorkOrder(int id)
    {
        var workOrder = await _context.WorkOrders
            .Include(w => w.Customer)
            .Include(w => w.AssignedTo)
            .Include(w => w.WorkOrderTasks)
            .Include(w => w.SiteVisitSignOffs)
            .Include(w => w.SalesOrder)
            .Include(w => w.Quote)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workOrder == null)
        {
            return NotFound();
        }

        return workOrder;
    }

    [HttpPost]
    public async Task<ActionResult<WorkOrder>> PostWorkOrder(WorkOrder workOrder)
    {
        workOrder.CreatedDate = DateTime.UtcNow;
        workOrder.WorkOrderDate = DateTime.UtcNow;
        
        _context.WorkOrders.Add(workOrder);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWorkOrder), new { id = workOrder.Id }, workOrder);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutWorkOrder(int id, WorkOrder workOrder)
    {
        if (id != workOrder.Id)
        {
            return BadRequest();
        }

        workOrder.LastModifiedDate = DateTime.UtcNow;
        
        _context.Entry(workOrder).State = EntityState.Modified;

        // Handle work order tasks
        var existingTasks = await _context.WorkOrderTasks.Where(t => t.WorkOrderId == id).ToListAsync();
        _context.WorkOrderTasks.RemoveRange(existingTasks);
        _context.WorkOrderTasks.AddRange(workOrder.WorkOrderTasks);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!WorkOrderExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkOrder(int id)
    {
        var workOrder = await _context.WorkOrders.FindAsync(id);
        if (workOrder == null)
        {
            return NotFound();
        }

        _context.WorkOrders.Remove(workOrder);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/tasks")]
    public async Task<ActionResult<WorkOrderTask>> AddTask(int id, WorkOrderTask task)
    {
        var workOrder = await _context.WorkOrders.FindAsync(id);
        if (workOrder == null)
        {
            return NotFound();
        }

        task.WorkOrderId = id;
        _context.WorkOrderTasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWorkOrder), new { id }, task);
    }

    [HttpPut("{workOrderId}/tasks/{taskId}")]
    public async Task<IActionResult> UpdateTask(int workOrderId, int taskId, WorkOrderTask task)
    {
        if (taskId != task.Id || workOrderId != task.WorkOrderId)
        {
            return BadRequest();
        }

        _context.Entry(task).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.WorkOrderTasks.Any(t => t.Id == taskId))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{workOrderId}/tasks/{taskId}")]
    public async Task<IActionResult> DeleteTask(int workOrderId, int taskId)
    {
        var task = await _context.WorkOrderTasks.FindAsync(taskId);
        if (task == null || task.WorkOrderId != workOrderId)
        {
            return NotFound();
        }

        _context.WorkOrderTasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool WorkOrderExists(int id)
    {
        return _context.WorkOrders.Any(e => e.Id == id);
    }
}
