namespace FocusFlow.Console;
using System;
public class TaskItem
{
    public required string Title { get; set; }
    public string Description { get; set; } = "No description";
    public bool IsCompleted { get; set; }
    public static List<TaskItem> Tasks = new List<TaskItem>();
    public DateTime? DueDate { get; set; }

    public static void AddTask()
    {
        Console.Write("Enter task title: ");
        string title = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Task title may not be empty.");
        }
        else
        {
            TaskItem task = new TaskItem { Title = title };
            
            Console.Write("Enter task description: ");
            string description = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(description))
                task.Description = description;
            
            Tasks.Add(task);
        }
    }

    public static void ShowTasks()
    {
        Console.WriteLine(" ---------  Tasks  ---------");
        if (Tasks.Count == 0)
            Console.WriteLine("No tasks found.");
        
        
        for (int i = 0; i < Tasks.Count; i++)
        {
            var status = Tasks[i].IsCompleted ? "[\u2713] Complete" : "[ ] Incomplete";
            Console.WriteLine($"{i + 1}. {status} | {Tasks[i].Title} - {Tasks[i].Description}");
        }
        
        Console.WriteLine();
    }

    public static void CompleteTask()
    {
        if (Tasks.Count == 0)
        {
            Console.WriteLine("No tasks found.");
        }
        else
        {
            Console.Write("Enter task number to complete: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= Tasks.Count)
            {
                Tasks[index - 1].IsCompleted = true;
            }
            else
            {
                Console.WriteLine("Invalid task number. Please try again.");
            }
        }
        

    }
    
}