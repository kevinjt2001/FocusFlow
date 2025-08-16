using Microsoft.EntityFrameworkCore;
using FocusFlow.API.Models;

namespace FocusFlow.API.Data;

public interface INoteRepository
{
    Task<List<NoteItem>> GetAllNotesAsync();
    Task<NoteItem?> GetNoteByIdAsync(Guid noteId);
    Task<List<NoteItem>> GetNotesByTaskIdAsync(Guid taskId);
    Task<NoteItem> AddNoteAsync(NoteItem note);
    Task<NoteItem?> UpdateNoteAsync(NoteItem note);
    Task<bool> DeleteNoteAsync(Guid noteId);
    Task SaveChangesAsync();
}

public class NoteRepository : INoteRepository
{
    private readonly FocusFlowDbContext _context;

    public NoteRepository(FocusFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<NoteItem>> GetAllNotesAsync()
    {
        return await _context.Notes
            .Include(n => n.LinkedTask)
            .ToListAsync();
    }

    public async Task<NoteItem?> GetNoteByIdAsync(Guid noteId)
    {
        return await _context.Notes
            .Include(n => n.LinkedTask)
            .FirstOrDefaultAsync(n => n.NoteID == noteId);
    }

    public async Task<List<NoteItem>> GetNotesByTaskIdAsync(Guid taskId)
    {
        return await _context.Notes
            .Include(n => n.LinkedTask)
            .Where(n => n.LinkedTaskID == taskId)
            .ToListAsync();
    }

    public async Task<NoteItem> AddNoteAsync(NoteItem note)
    {
        _context.Notes.Add(note);
        await SaveChangesAsync();
        return note;
    }

    public async Task<NoteItem?> UpdateNoteAsync(NoteItem note)
    {
        var existingNote = await GetNoteByIdAsync(note.NoteID);
        if (existingNote == null) return null;

        existingNote.Title = note.Title;
        existingNote.Content = note.Content;
        existingNote.UpdatedAt = DateTime.UtcNow;
        existingNote.LinkedTaskID = note.LinkedTaskID;

        await SaveChangesAsync();
        return existingNote;
    }

    public async Task<bool> DeleteNoteAsync(Guid noteId)
    {
        var note = await GetNoteByIdAsync(noteId);
        if (note == null) return false;

        _context.Notes.Remove(note);
        await SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}