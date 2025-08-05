namespace FocusFlow;
using System.Text.Json;

public class DataManager
{
    // The name of the file where tasks are saved/loaded
    private const string FileName = "tasks.json";

    /// <summary>
    /// Loads tasks from the "tasks.json" file.
    /// If the file doesn't exist, returns an empty task list.
    /// </summary>
    /// <returns>A list of TaskItem objects loaded from the file</returns>
    public static List<TaskItem> LoadTasks()
    {
        // If the file doesn't exist, return an empty list (no saved tasks)
        if (!File.Exists(FileName))
            return new List<TaskItem>();

        // Read the entire contents of the file as a JSON string
        var json = File.ReadAllText(FileName);

        // Deserialize the JSON into a list of TaskItem objects.
        // If deserialization fails or returns null, fallback to an empty list.
        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
    }
    
    /// <summary>
    /// Saves the provided list of tasks to the "tasks.json" file in a readable format.
    /// </summary>
    /// <param name="tasks">The list of TaskItem objects to save</param>
    public static void SaveTasks(List<TaskItem> tasks)
    {
        // Convert the task list to a formatted JSON string (with indentation for readability)
        var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });

        // Write the JSON string to the file, overwriting any existing content
        File.WriteAllText(FileName, json);
    }
    
}
