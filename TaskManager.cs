using System.Globalization;

namespace FocusFlow.Console;
using System;

public class TaskManager
{
    public List<TaskItem> Tasks { get; set; }

    public TaskManager()
    {
        Tasks = DataManager.LoadTasks();
    }

    public bool CheckForTasks(string msg)
    {
        if (Tasks.Count == 0)
        {
            Console.WriteLine(msg);
            //Console.WriteLine("No tasks found.");
            return false;
        }
        return true;
    }
    
    public void AddTask()
    {
        Console.Write("Enter task title: ");
        string title = Console.ReadLine().Trim();
        
        // Verify title is not null/empty/whitespace. Title must hold a value
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("\nTask title may not be empty.");
        }
        else
        {
            // Create a TaskItem object after title has been set
            TaskItem task = new TaskItem { Title = title };
            
            Console.Write("Enter task description (optional): ");
            string description = Console.ReadLine().Trim();

            // If user inputs a description, add it to the existing TaskItem. Description can be null/empty/whitespace
            if (!string.IsNullOrWhiteSpace(description))
                task.Description = description;
            
            // Get user input for DueDate and validate it. DueDate can be null.
            Console.Write("Enter task due date (optional, MM/dd/yyyy): ");
            DateTime? dueDate;

            while (true)
            {
                string userDate = Console.ReadLine();
                dueDate = ValidateDueDate(userDate);
    
                if (dueDate != null || string.IsNullOrWhiteSpace(userDate))
                    break;
    
                Console.Write("Please re-enter due date (MM/dd/yyyy) or leave blank: ");
            }
            task.DueDate = dueDate;
            
            Tasks.Add(task);
        }
        DataManager.SaveTasks(Tasks);
    }

    public void ShowTasks()
    {
        // Task display
        Console.WriteLine("\n---------  Tasks  ---------");
        if (!CheckForTasks("No tasks yet."))
        {
            Console.WriteLine();
            return;
        }
        
        // Loop through tasks and display them
        for (int i = 0; i < Tasks.Count; i++)
        {
            var status = Tasks[i].IsCompleted ? "[\u2713] Complete" : "[ ] Incomplete";
            var dueDateDisplay = Tasks[i].DueDate?.ToString("MM/dd/yyyy") ?? "No due date";
            Console.WriteLine($"{i + 1}. {status} | {Tasks[i].Title} - {Tasks[i].Description} - (Due: {dueDateDisplay})");
        }
        
        Console.WriteLine();
    }

    public void CompleteTask()
    {
        if (!CheckForTasks("No tasks to complete.")) return;
            
        // Mark task complete by task number (index)
        Console.Write("Enter task number to complete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= Tasks.Count)
        {
            Tasks[index - 1].IsCompleted = true;
        }
        else
        {
            Console.WriteLine("\nInvalid task number. Please try again.");
        }
        DataManager.SaveTasks(Tasks);
    }

    public void DeleteTask()
    {
        if (!CheckForTasks("No tasks to delete.")) return;
        
        // Delete task by number (index)
        Console.Write("Enter task number to delete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= Tasks.Count)
        {
            var deletedTask = Tasks[index - 1];
            Tasks.RemoveAt(index - 1);
            Console.WriteLine($"Task deleted: {deletedTask.Title}");
            // Update tasks.json
            DataManager.SaveTasks(Tasks);
        }
        else
        {
            Console.WriteLine("\nInvalid task number. Please try again.");
        }
    }
    
    // Method to handle DueDate
    public DateTime? ValidateDueDate(string userDate)
    {
        // Handle user input for due date. Due date can be null 
        if (string.IsNullOrWhiteSpace(userDate))
            return null;

        // Validate that the user input for due date is in proper format
        if (DateTime.TryParseExact(userDate.Trim(), "MM/dd/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate.Date;
        }
        
        // Error 
        Console.WriteLine("\nInvalid date format");
        return null;
    }
    
}