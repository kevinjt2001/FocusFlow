using FocusFlow.ConsoleApp.Models;

namespace FocusFlow.ConsoleApp.Services;

public class NoteManager
{
    private readonly List<NoteItem> _notes = new();
    private readonly TaskManager _taskManager;

    public NoteManager(TaskManager taskManager)
    {
        _taskManager = taskManager;
    }

    public List<NoteItem> GetAllNotes() => _notes;

    public NoteItem CreateNote(string title, string? content, Guid? linkedTaskID)
    {
        if (linkedTaskID.HasValue && !_taskManager.GetAllTasks().Any(t => t.ID == linkedTaskID.Value))
            throw new ArgumentException("Cannot link note to a non-existent task.");

        var note = new NoteItem()
        {
            Title = title,
            Content = content ?? string.Empty,
            LinkedTaskID = linkedTaskID
        };
        
        _notes.Add(note);
        return note;
    }
    
}