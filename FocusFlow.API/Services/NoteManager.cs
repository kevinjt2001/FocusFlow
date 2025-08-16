using FocusFlow.API.Data;
using FocusFlow.API.Models;

namespace FocusFlow.API.Services;

public class NoteManager
{
    private readonly INoteRepository _noteRepository;
    private readonly ITaskRepository _taskRepository;

    public NoteManager(INoteRepository noteRepository, ITaskRepository taskRepository)
    {
        _noteRepository = noteRepository;
        _taskRepository = taskRepository;
    }

    public async Task<List<NoteItem>> GetAllNotesAsync()
    {
        return await _noteRepository.GetAllNotesAsync();
    }

    public async Task<NoteItem?> GetNoteByIdAsync(Guid noteId)
    {
        return await _noteRepository.GetNoteByIdAsync(noteId);
    }

    public async Task<List<NoteItem>> GetNotesByTaskIdAsync(Guid taskId)
    {
        return await _noteRepository.GetNotesByTaskIdAsync(taskId);
    }

    public async Task<NoteItem?> CreateNoteAsync(string title, string? content, Guid? linkedTaskId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return null;
        
        if (linkedTaskId.HasValue)
        {
            var task = await _taskRepository.GetTaskByIdAsync(linkedTaskId.Value);
            if (task == null)
                return null; 
        }

        var note = new NoteItem
        {
            Title = title.Trim(),
            Content = content?.Trim() ?? string.Empty,
            LinkedTaskID = linkedTaskId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _noteRepository.AddNoteAsync(note);
    }

    public async Task<NoteItem?> UpdateNoteAsync(Guid noteId, string? newTitle = null, string? newContent = null, Guid? newLinkedTaskId = null)
    {
        var note = await _noteRepository.GetNoteByIdAsync(noteId);
        if (note == null) return null;
        
        if (newLinkedTaskId.HasValue && newLinkedTaskId != note.LinkedTaskID)
        {
            var task = await _taskRepository.GetTaskByIdAsync(newLinkedTaskId.Value);
            if (task == null)
                return null; 
        }

        if (!string.IsNullOrWhiteSpace(newTitle))
            note.Title = newTitle.Trim();

        if (newContent != null)
            note.Content = newContent.Trim();

        if (newLinkedTaskId != note.LinkedTaskID)
            note.LinkedTaskID = newLinkedTaskId;

        note.UpdatedAt = DateTime.UtcNow;

        return await _noteRepository.UpdateNoteAsync(note);
    }

    public async Task<bool> DeleteNoteAsync(Guid noteId)
    {
        return await _noteRepository.DeleteNoteAsync(noteId);
    }

    public async Task<bool> LinkNoteToTaskAsync(Guid noteId, Guid taskId)
    {
        var note = await _noteRepository.GetNoteByIdAsync(noteId);
        var task = await _taskRepository.GetTaskByIdAsync(taskId);

        if (note == null || task == null) return false;

        note.LinkedTaskID = taskId;
        note.UpdatedAt = DateTime.UtcNow;

        await _noteRepository.UpdateNoteAsync(note);
        return true;
    }

    public async Task<bool> UnlinkNoteFromTaskAsync(Guid noteId)
    {
        var note = await _noteRepository.GetNoteByIdAsync(noteId);
        if (note == null) return false;

        note.LinkedTaskID = null;
        note.UpdatedAt = DateTime.UtcNow;

        await _noteRepository.UpdateNoteAsync(note);
        return true;
    }
}