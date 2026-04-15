namespace TimeCrm.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeCrm.Data;
using TimeCrm.Models;
using TimeCrm.DTOs;

[ApiController]
[Route("api/[controller]")]
public class TimeEntriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TimeEntriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimeEntry>>> GetTimeEntries(
        [FromQuery] DateOnly? date = null,
        [FromQuery] int? month = null,
        [FromQuery] int? year = null)
    {
        var query = _context.TimeEntries
            .Include(te => te.Task)
            .ThenInclude(t => t.Project)
            .AsQueryable();

        if (date.HasValue)
        {
            query = query.Where(te => te.Date == date.Value);
        }
        else if (month.HasValue && year.HasValue)
        {
            query = query.Where(te => te.Date.Month == month && te.Date.Year == year);
        }

        return await query.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TimeEntry>> GetTimeEntry(int id)
    {
        var entry = await _context.TimeEntries
            .Include(te => te.Task)
            .ThenInclude(t => t.Project)
            .FirstOrDefaultAsync(te => te.Id == id);

        if (entry == null)
        {
            return NotFound();
        }
        return entry;
    }

    [HttpPost]
    public async Task<ActionResult<TimeEntry>> CreateTimeEntry(TimeEntryDto dto)
    {
        // 1. Verify task exists and is active
        var task = await _context.Tasks.FindAsync(dto.TaskId);
        if (task == null || !task.IsActive)
        {
            return BadRequest("Selected task is not active or does not exist.");
        }

        // 2. Check daily hour limit
        var totalHoursForDay = await _context.TimeEntries
            .Where(te => te.Date == dto.Date)
            .SumAsync(te => te.Hours);

        if (totalHoursForDay + dto.Hours > 24)
        {
            return BadRequest("Total hours for the day cannot exceed 24.");
        }

        var entry = new TimeEntry
        {
            Date = dto.Date,
            Hours = dto.Hours,
            Description = dto.Description,
            TaskId = dto.TaskId,
            TaskWasActiveAtCreation = task.IsActive
        };

        _context.TimeEntries.Add(entry);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTimeEntry), new { id = entry.Id }, entry);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTimeEntry(int id, TimeEntryDto dto)
    {
        var entry = await _context.TimeEntries.FindAsync(id);
        if (entry == null)
        {
            return NotFound();
        }

        // Check if task can be changed
        if (entry.TaskId != dto.TaskId)
        {
            var originalTask = await _context.Tasks.FindAsync(entry.TaskId);
            if (originalTask != null && !entry.TaskWasActiveAtCreation)
            {
                return BadRequest("Cannot change task because the original task is now inactive.");
            }

            var newTask = await _context.Tasks.FindAsync(dto.TaskId);
            if (newTask == null || !newTask.IsActive)
            {
                return BadRequest("New task is not active.");
            }
        }

        // Check daily hour limit
        var totalHoursForDay = await _context.TimeEntries
            .Where(te => te.Date == dto.Date && te.Id != id)
            .SumAsync(te => te.Hours);

        if (totalHoursForDay + dto.Hours > 24)
        {
            return BadRequest("Total hours for the day cannot exceed 24.");
        }

        entry.Date = dto.Date;
        entry.Hours = dto.Hours;
        entry.Description = dto.Description;
        entry.TaskId = dto.TaskId;

        // Update flag if task changed
        if (entry.TaskId != dto.TaskId)
        {
            var task = await _context.Tasks.FindAsync(dto.TaskId);
            entry.TaskWasActiveAtCreation = task?.IsActive ?? false;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTimeEntry(int id)
    {
        var entry = await _context.TimeEntries.FindAsync(id);
        if (entry == null)
        {
            return NotFound();
        }

        _context.TimeEntries.Remove(entry);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("daily-summary")]
    public async Task<ActionResult<DailySummaryDto>> GetDailySummary([FromQuery] DateOnly date)
    {
        var totalHours = await _context.TimeEntries
            .Where(te => te.Date == date)
            .SumAsync(te => te.Hours);

        string color;
        if (totalHours < 8)
            color = "yellow";
        else if (totalHours == 8)
            color = "green";
        else
            color = "red";

        return new DailySummaryDto
        {
            Date = date,
            TotalHours = totalHours,
            Color = color
        };
    }
}