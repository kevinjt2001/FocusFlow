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
            task.DueDate = HandleDueDate();
            
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
            if (Tasks[index - 1].IsCompleted)
            {
                Console.WriteLine("\nTask is already complete.");
            }
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

    public DateTime? HandleDueDate()
    {
        DateTime? dueDate;
        while (true)
        {
            string userDate = Console.ReadLine();
            dueDate = ValidateDueDate(userDate);
    
            if (dueDate != null || string.IsNullOrWhiteSpace(userDate))
                break;
    
            Console.Write("Please re-enter due date (MM/dd/yyyy) or leave blank: ");
        }

        return dueDate;
    }

    /* EditTask() still needs work:
        - Title must not be null
        - Proper checks of (y/n) - currently allows for no input
        - Still need to save tasks with DataManager after edits are made
        - Perhaps more....
     */
    public void EditTask()
    {
        if (!CheckForTasks("No tasks to edit.")) return;
        
        Console.Write("Enter task number to edit: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= Tasks.Count)
        {
            var task = Tasks[index - 1];

            var taskStatus = task.IsCompleted ? "[\u2713] Complete" : "[ ] Incomplete";
            Console.WriteLine($"Task is {taskStatus}");
            Console.Write("Would you like to edit this status? (y/n): ");
            if (Console.ReadLine().Trim().ToLower() == "y")
            {
                while (true)
                {
                    Console.Write("Mark task (Complete/Incomplete): ");
                    string newStatus = Console.ReadLine().Trim().ToLower();
                    if (newStatus == "complete")
                    {
                        task.IsCompleted = true;
                        break;
                    }
                    
                    if (newStatus == "incomplete")
                    {
                        task.IsCompleted = false;
                        break;
                    }
                    
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
            
            Console.WriteLine($"Current title: {task.Title}");
            Console.Write("Would you like to edit this title? (y/n): ");
            if (Console.ReadLine().Trim().ToLower() == "y")
            {
                Console.Write("Enter new title: ");
                task.Title = Console.ReadLine().Trim();
            }

            Console.WriteLine($"Current description: {task.Description}");
            Console.Write("Would you like to edit this description? (y/n): ");
            if (Console.ReadLine().Trim().ToLower() == "y")
            {
                Console.Write("Enter new description: ");
                task.Description = Console.ReadLine().Trim();
            }
            
            Console.WriteLine($"Current due date: {task.DueDate?.ToString("MM/dd/yyyy") ?? "No due date"}");
            Console.Write("Would you like to edit this due date? (y/n): ");
            if (Console.ReadLine().Trim().ToLower() == "y")
            {
                Console.Write("Enter new date (MM/dd/yyyy): ");
                task.DueDate = HandleDueDate();
            }

        }
        else
        {
            Console.WriteLine("\nInvalid task number. Please try again.");
        }
    }
    
}