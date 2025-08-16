using System.Globalization;
using FocusFlow.API.Data;
using FocusFlow.API.Models;

namespace FocusFlow.API.Services;

public class TaskManager
{
    private readonly ITaskRepository _taskRepository;
    private string? _currentFilter;
    private string? _sortOrder;

    public TaskManager(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    // Basic CRUD operations
    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        return await _taskRepository.GetAllTasksAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid taskId)
    {
        return await _taskRepository.GetTaskByIdAsync(taskId);
    }

    // Get visible tasks with current filters and sorting applied
    public async Task<List<TaskItem>> GetVisibleTasksAsync()
    {
        bool? completionFilter = _currentFilter switch
        {
            "complete" => true,
            "incomplete" => false,
            _ => null
        };

        return await _taskRepository.GetFilteredAndSortedTasksAsync(completionFilter, _sortOrder);
    }

    public async Task<List<TaskItem>> GetTasksByPriorityAsync(Priority priority)
    {
        return await _taskRepository.GetTasksByPriorityAsync(priority);
    }

    public async Task<List<TaskItem>> GetTasksByCompletionStatusAsync(bool isCompleted)
    {
        return await _taskRepository.GetTasksByCompletionStatusAsync(isCompleted);
    }

    public async Task<TaskItem?> AddTaskAsync(string title, string? description, DateTime? dueDate, Priority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            return null;

        var task = new TaskItem
        {
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? "No description" : description.Trim(),
            DueDate = dueDate,
            Priority = priority
        };

        return await _taskRepository.AddTaskAsync(task);
    }

    public async Task<TaskItem?> UpdateTaskAsync(Guid taskId, string? newTitle = null, string? newDescription = null,
        DateTime? newDueDate = null, bool? newStatus = null, Priority? newPriority = null)
    {
        var task = await _taskRepository.GetTaskByIdAsync(taskId);
        if (task == null) return null;

        if (!string.IsNullOrWhiteSpace(newTitle))
            task.Title = newTitle.Trim();

        if (newDescription != null)
            task.Description = newDescription.Trim();

        if (newDueDate.HasValue)
            task.DueDate = newDueDate;

        if (newStatus.HasValue)
            task.IsCompleted = newStatus.Value;

        if (newPriority.HasValue)
            task.Priority = newPriority.Value;

        return await _taskRepository.UpdateTaskAsync(task);
    }

    public async Task<bool> CompleteTaskAsync(Guid taskId)
    {
        var task = await _taskRepository.GetTaskByIdAsync(taskId);
        if (task == null || task.IsCompleted) return false;

        task.IsCompleted = true;
        await _taskRepository.UpdateTaskAsync(task);
        return true;
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        return await _taskRepository.DeleteTaskAsync(taskId);
    }
    
    public bool FilterByStatus(string status)
    {
        if (status != "complete" && status != "incomplete") return false;
        _currentFilter = status;
        return true;
    }

    public void ClearFilter()
    {
        _currentFilter = null;
    }

    public bool SortByDueDate(string order)
    {
        if (order != "oldest" && order != "newest") return false;
        _sortOrder = order;
        return true;
    }

    public void ClearSort()
    {
        _sortOrder = null;
    }
    
    public async Task<List<string>> GetVisibleTaskDescriptionsAsync()
    {
        var tasks = await GetVisibleTasksAsync();
        return tasks.Select(t =>
        {
            var status = t.IsCompleted ? "[✓] Complete" : "[ ] Incomplete";
            var due = t.DueDate?.ToString("MM/dd/yyyy") ?? "No due date";
            var priority = t.Priority.ToString();
            return $"{status} | {t.Title} - {t.Description} - (Due: {due}) - (Priority: {priority})";
        }).ToList();
    }
    
    public static DateTime? ParseDueDate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        return DateTime.TryParseExact(input.Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var date)
            ? date.Date
            : null;
    }

    public static Priority? ParsePriority(string? priorityString)
    {
        if (string.IsNullOrWhiteSpace(priorityString)) return null;

        return priorityString.Trim().ToLower() switch
        {
            "low" => Priority.Low,
            "medium" => Priority.Medium,
            "high" => Priority.High,
            _ => null
        };
    }

    public static bool IsValidPriority(string? priority)
    {
        return ParsePriority(priority).HasValue;
    }
}