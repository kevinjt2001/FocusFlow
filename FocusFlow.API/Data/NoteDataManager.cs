using System.Text.Json;
using FocusFlow.API.Models;

namespace FocusFlow.API.Data;

public class NoteDataManager
{
    private readonly string _filePath;

    public NoteDataManager(string filePath = "notes.json")
    {
        _filePath = filePath;
    }

    public List<NoteItem> LoadNotes()
    {
        if (!File.Exists(_filePath))
            return new List<NoteItem>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<NoteItem>>(json) ?? new List<NoteItem>();
    }

    public void SaveNotes(List<NoteItem> notes)
    {
        var json = JsonSerializer.Serialize(notes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}