
namespace FocusFlow.ConsoleApp.Models;

public class TaskItem
{
    public Guid TaskID { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string Description { get; set; } = "No description";
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    

}