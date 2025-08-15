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

    public NoteItem CreateStandaloneNote(string title, string? content)
    {
        return CreateNote(title, content, null);
    }

    public NoteItem CreateLinkedNote(string title, string? content, Guid linkedTaskID)
    {
        if (_taskManager.GetTaskByID(linkedTaskID) == null)
            throw new ArgumentException("Cannot link note to a non-existent task.");

        return CreateNote(title, content, linkedTaskID);
    }
    

    public NoteItem CreateNote(string title, string? content, Guid? linkedTaskID)
    {
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