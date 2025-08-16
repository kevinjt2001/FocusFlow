namespace FocusFlow.API.Models;

public class NoteItem
{
    public Guid NoteID { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Content { get; set; } = string.Empty;
    
    public Guid? LinkedTaskID { get; set; }
    public bool IsLinkedToTask => LinkedTaskID.HasValue;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}