using Microsoft.EntityFrameworkCore;
using FocusFlow.API.Models;

namespace FocusFlow.API.Data;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllTasksAsync();
    Task<TaskItem?> GetTaskByIdAsync(Guid taskId);
    Task<List<TaskItem>> GetTasksByPriorityAsync(Priority priority);
    Task<List<TaskItem>> GetTasksByCompletionStatusAsync(bool isCompleted);
    Task<List<TaskItem>> GetFilteredAndSortedTasksAsync(bool? completionFilter = null, string? sortOrder = null);
    Task<TaskItem> AddTaskAsync(TaskItem task);
    Task<TaskItem?> UpdateTaskAsync(TaskItem task);
    Task<bool> DeleteTaskAsync(Guid taskId);
    Task SaveChangesAsync();
}

public class TaskRepository : ITaskRepository
{
    private readonly FocusFlowDbContext _context;

    public TaskRepository(FocusFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        return await _context.Tasks
            .Include(t => t.LinkedNotes)
            .OrderBy(t => t.Priority)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid taskId)
    {
        return await _context.Tasks
            .Include(t => t.LinkedNotes)
            .FirstOrDefaultAsync(t => t.TaskID == taskId);
    }

    public async Task<List<TaskItem>> GetTasksByPriorityAsync(Priority priority)
    {
        return await _context.Tasks
            .Include(t => t.LinkedNotes)
            .Where(t => t.Priority == priority)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetTasksByCompletionStatusAsync(bool isCompleted)
    {
        return await _context.Tasks
            .Include(t => t.LinkedNotes)
            .Where(t => t.IsCompleted == isCompleted)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetFilteredAndSortedTasksAsync(bool? completionFilter = null, string? sortOrder = null)
    {
        var query = _context.Tasks.Include(t => t.LinkedNotes).AsQueryable();

        // Apply completion status filter
        if (completionFilter.HasValue)
        {
            query = query.Where(t => t.IsCompleted == completionFilter.Value);
        }

        // Apply sorting by due date
        if (!string.IsNullOrEmpty(sortOrder))
        {
            query = sortOrder.ToLower() switch
            {
                "oldest" => query.OrderBy(t => t.DueDate ?? DateTime.MaxValue),
                "newest" => query.OrderByDescending(t => t.DueDate ?? DateTime.MinValue),
                _ => query.OrderBy(t => t.Priority) // Default sort by priority
            };
        }
        else
        {
            query = query.OrderBy(t => t.Priority); // Default sort by priority
        }

        return await query.ToListAsync();
    }

    public async Task<TaskItem> AddTaskAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateTaskAsync(TaskItem task)
    {
        var existingTask = await GetTaskByIdAsync(task.TaskID);
        if (existingTask == null) return null;

        _context.Entry(existingTask).CurrentValues.SetValues(task);
        await SaveChangesAsync();
        return existingTask;
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        var task = await GetTaskByIdAsync(taskId);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}