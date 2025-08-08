using System.Text.Json;

namespace FocusFlow.ConsoleApp;

public class DataManager : IDataManager
{
    private readonly string _filePath;

    public DataManager(string filePath = "tasks.json")
    {
        _filePath = filePath;
    }

    public List<TaskItem> LoadTasks()
    {
        if (!File.Exists(_filePath))
            return new List<TaskItem>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
    }

    public void SaveTasks(List<TaskItem> tasks)
    {
        var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}