using System.ComponentModel.DataAnnotations;

namespace FocusFlow.API.Models;

public class NoteItem
{
    [Key]
    public Guid NoteID { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public required string Title { get; set; }
    
    [MaxLength(5000)]
    public string? Content { get; set; } = string.Empty;
    
    // Foreign key to TaskItem
    public Guid? LinkedTaskID { get; set; }
    public bool IsLinkedToTask => LinkedTaskID.HasValue;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // EF Core navigation property
    public virtual TaskItem? LinkedTask { get; set; }
}