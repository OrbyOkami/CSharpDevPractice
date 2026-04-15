using TimeCrm.DTOs;

namespace TimeCrm.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeCrm.Data;
using TimeCrm.Models;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
    {
        return await _context.Tasks.Include(t => t.Project).ToListAsync();
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetActiveTasks()
    {
        return await _context.Tasks
            .Where(t => t.IsActive)
            .Include(t => t.Project)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }
        return task;
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask(CreateTaskDto dto)
    {
        // Проверка существования проекта
        var project = await _context.Projects.FindAsync(dto.ProjectId);
        if (project == null)
            return BadRequest("Project not found.");

        var task = new TaskItem
        {
            Name = dto.Name,
            ProjectId = dto.ProjectId,
            IsActive = dto.IsActive
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, TaskItem task)
    {
        if (id != task.Id)
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
            if (!await _context.Tasks.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        // Check for related time entries
        if (await _context.TimeEntries.AnyAsync(te => te.TaskId == id))
        {
            return BadRequest("Cannot delete task with existing time entries.");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}