using System.Globalization;
using System.Net;

namespace FocusFlow.Console;
using System;
public class TaskItem
{
    public required string Title { get; set; }
    public string Description { get; set; } = "No description";
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }

}