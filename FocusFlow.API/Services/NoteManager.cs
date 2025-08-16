using FocusFlow.API.Data;
using FocusFlow.API.Models;

namespace FocusFlow.API.Services;

public class NoteManager
{
    private readonly List<NoteItem> _notes = new();
    private readonly TaskManager _taskManager;
    private readonly NoteDataManager _noteDataManager;

    public NoteManager(TaskManager taskManager, NoteDataManager noteDataManager)
    {
        _taskManager = taskManager;
        _noteDataManager = noteDataManager;
        _notes = _noteDataManager.LoadNotes();
    }
    public List<NoteItem> GetAllNotes() => _notes;
    public NoteItem? GetNoteByID(Guid noteID) => _notes.FirstOrDefault(n => n.NoteID == noteID);
    public List<NoteItem> GetNotesByTask(Guid taskID) => _notes.Where(n => n.LinkedTaskID == taskID).ToList();
    
    public NoteItem CreateNote(string title, string? content, Guid? linkedTaskID = null)
    {
        var note = new NoteItem()
        {
            Title = title,
            Content = content ?? string.Empty,
            LinkedTaskID = linkedTaskID
        };
        
        _notes.Add(note);
        _noteDataManager.SaveNotes(_notes);
        return note;
    }

    public bool UpdateNote(Guid noteID, string? newTitle = null, string? newContent = null)
    {
        var note = GetNoteByID(noteID);

        if (note == null)
            return false;

        if (!string.IsNullOrWhiteSpace(newTitle))
            note.Title = newTitle;

        if (!string.IsNullOrWhiteSpace(newContent))
            note.Content = newContent;
        
        _noteDataManager.SaveNotes(_notes);
        return true;
    }

    public bool DeleteNote(Guid noteID)
    {
        var note = GetNoteByID(noteID);
        
        if (note == null)
            return false;
        
        _notes.Remove(note);
        _noteDataManager.SaveNotes(_notes);
        return true;
    }
    
    
}