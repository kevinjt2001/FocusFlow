namespace FocusFlow.ConsoleApp.Models;

public class NoteItem
{
    public required string Title { get; set; }
    public string? Content { get; set; } = string.Empty;

    public Guid ID { get; set; } = Guid.NewGuid();
    public Guid? LinkedTaskID { get; set; }
    public bool IsLinkedToTask => LinkedTaskID.HasValue;
    
    
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}