namespace FocusFlow.Console;
using System.Text.Json;

public class DataManager
{
    private const string FileName = "tasks.json";

    public static List<TaskItem> LoadTasks()
    {
        if (!File.Exists(FileName))
            return new List<TaskItem>();
        
        var json = File.ReadAllText(FileName);
        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
    }
    
    public static void SaveTasks(List<TaskItem> tasks)
    {
        var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FileName, json);
    }
    
}