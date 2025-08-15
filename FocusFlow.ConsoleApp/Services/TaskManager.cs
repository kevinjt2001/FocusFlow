using System.Globalization;
using FocusFlow.ConsoleApp.Data;
using FocusFlow.ConsoleApp.Models;

namespace FocusFlow.ConsoleApp.Services;

public class TaskManager
{
    private readonly IDataManager _dataManager;
    public List<TaskItem> Tasks { get; set; }
    public List<TaskItem> VisibleTasks { get; private set; }
    private string? CurrentFilter = null;
    private string? SortOrder = null;

    public TaskManager(IDataManager dataManager)
    {
        _dataManager = dataManager;
        Tasks = _dataManager.LoadTasks();
        VisibleTasks = new List<TaskItem>(Tasks);
    }
    
    public void ClearFilter() => CurrentFilter = null;

    public void ClearSort() => SortOrder = null;

    public bool AddTask(string title, string? description, DateTime? dueDate, string priority)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(priority))
            return false;

        var task = new TaskItem
        {
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? "No description" : description.Trim(),
            DueDate = dueDate,
            Priority = priority.Trim().ToLower()
        };

        Tasks.Add(task);
        _dataManager.SaveTasks(Tasks);
        return true;
    }

    public List<string> GetVisibleTaskDescriptions()
    {
        ApplyFilterAndSort();
        return VisibleTasks.Select(t =>
        {
            var status = t.IsCompleted ? "[✓] Complete" : "[ ] Incomplete";
            var due = t.DueDate?.ToString("MM/dd/yyyy") ?? "No due date";
            var priority = t.Priority ?? "No priority";
            return $"{status} | {t.Title} - {t.Description} - (Due: {due}) - (Priority: {priority})";
        }).ToList();
    }

    public bool CompleteTask(int index)
    {
        if (!IsValidIndex(index)) return false;

        var task = VisibleTasks[index - 1];
        if (task.IsCompleted) return false;

        task.IsCompleted = true;
        _dataManager.SaveTasks(Tasks);
        return true;
    }

    public bool DeleteTask(int index)
    {
        if (!IsValidIndex(index)) return false;

        var task = VisibleTasks[index - 1];
        Tasks.Remove(task);
        _dataManager.SaveTasks(Tasks);
        return true;
    }

    public bool EditTask(int index, string? newTitle = null, string? newDescription = null,
        DateTime? newDueDate = null, bool? newStatus = null, string? newPriority = null)
    {
        if (!IsValidIndex(index)) return false;

        var task = VisibleTasks[index - 1];

        if (!string.IsNullOrWhiteSpace(newTitle))
            task.Title = newTitle.Trim();

        if (newDescription != null)
            task.Description = newDescription.Trim();

        if (newDueDate.HasValue)
            task.DueDate = newDueDate;

        if (newStatus.HasValue)
            task.IsCompleted = newStatus.Value;

        if (!string.IsNullOrWhiteSpace(newPriority))
            task.Priority = newPriority.Trim().ToLower();

        _dataManager.SaveTasks(Tasks);
        return true;
    }

    public bool FilterByStatus(string status)
    {
        if (status != "complete" && status != "incomplete") return false;

        CurrentFilter = status;
        return true;
    }

    public bool SortByDueDate(string order)
    {
        if (order != "oldest" && order != "newest") return false;

        SortOrder = order;
        return true;
    }

    public static DateTime? ParseDueDate(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        return DateTime.TryParseExact(input.Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var date)
            ? date.Date
            : null;
    }

    public static bool IsValidPriority(string priority)
    {
        return priority is "low" or "medium" or "high";
    }

    private void ApplyFilterAndSort()
    {
        var filtered = string.IsNullOrEmpty(CurrentFilter)
            ? Tasks
            : Tasks.Where(t => CurrentFilter == "complete" ? t.IsCompleted : !t.IsCompleted).ToList();

        if (!string.IsNullOrEmpty(SortOrder))
        {
            filtered = SortOrder == "oldest"
                ? filtered.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ToList()
                : filtered.OrderByDescending(t => t.DueDate ?? DateTime.MinValue).ToList();
        }

        VisibleTasks = filtered;
    }

    private bool IsValidIndex(int index) => index >= 1 && index <= VisibleTasks.Count;
}
