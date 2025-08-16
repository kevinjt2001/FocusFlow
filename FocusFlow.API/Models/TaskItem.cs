using System.ComponentModel.DataAnnotations;

namespace FocusFlow.API.Models;

public class TaskItem
{
    [Key]
    public Guid TaskID { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public required string Title { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; } = "No description";
    
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    
    [Required]
    public Priority Priority { get; set; } = Priority.Medium; 
    
    public virtual ICollection<NoteItem> LinkedNotes { get; set; } = new List<NoteItem>();
}