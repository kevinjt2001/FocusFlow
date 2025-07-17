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

    public void ShowTasks(List<TaskItem> showTasks)
    {
        // Task display
        Console.WriteLine("\n---------  Tasks  ---------");
        if (!CheckForTasks("No tasks yet."))
        {
            Console.WriteLine();
            return;
        }
        
        // Loop through tasks and display them
        for (int i = 0; i < showTasks.Count; i++)
        {
            var status = showTasks[i].IsCompleted ? "[\u2713] Complete" : "[ ] Incomplete";
            var dueDateDisplay = showTasks[i].DueDate?.ToString("MM/dd/yyyy") ?? "No due date";
            Console.WriteLine($"{i + 1}. {status} | {showTasks[i].Title} - {showTasks[i].Description} - (Due: {dueDateDisplay})");
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
    
    public bool CheckYesOrNo(string msg)
    {
        while (true)
        {
            Console.Write(msg);
            string userInput = Console.ReadLine().Trim().ToLower();
            
            if (userInput == "y" || userInput == "yes") return true;
            if (userInput == "n" || userInput == "no") return false;
            
            Console.WriteLine("\nInvalid input. Please enter 'y' or 'n'.");
        }
    }
    public void EditTask()
    {
        if (!CheckForTasks("No tasks to edit.")) return;
        
        Console.Write("Enter task number to edit: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= Tasks.Count)
        {
            var task = Tasks[index - 1];

            var taskStatus = task.IsCompleted ? "[\u2713] Complete" : "[ ] Incomplete";
            Console.WriteLine($"\nTask is {taskStatus}");
            
            if (CheckYesOrNo("Would you like to edit this status? (y/n): "))
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
                    Console.WriteLine("\nInvalid input. Please try again.");
                }
            }
            
            Console.WriteLine($"\nCurrent title: {task.Title}");
            if (CheckYesOrNo("Would you like to edit this title? (y/n): "))
            {
                while (true)
                {
                    Console.Write("Enter new title: ");
                    string newTitle = Console.ReadLine().Trim();
                    if (!string.IsNullOrWhiteSpace(newTitle))
                    {
                        task.Title = newTitle;
                        break;
                    }
                    
                    Console.WriteLine("\nTask title may not be empty. Please try again.");
                       
                }
                
            }

            Console.WriteLine($"\nCurrent description: {task.Description}");
            if (CheckYesOrNo("Would you like to edit this description? (y/n): "))
            {
                Console.Write("Enter new description: ");
                task.Description = Console.ReadLine().Trim();
            }
            
            Console.WriteLine($"\nCurrent due date: {task.DueDate?.ToString("MM/dd/yyyy") ?? "No due date"}");
            if (CheckYesOrNo("Would you like to edit this due date? (y/n): "))
            {
                Console.Write("Enter new date (MM/dd/yyyy): ");
                task.DueDate = HandleDueDate();
            }
            
            DataManager.SaveTasks(Tasks);
            Console.WriteLine("\nTask updated successfully.");

        }
        else
        {
            Console.WriteLine("\nInvalid task number. Please try again.");
        }
    }
    
    public void FilterByStatus()
    {
        if (!CheckForTasks("No tasks to filter.")) return;
        
        List<TaskItem> filterList = DataManager.LoadTasks();
        Console.Write("Filter tasks by status (complete/incomplete): ");
        string userInput = Console.ReadLine().Trim().ToLower();
        
        if (userInput == "complete")
        {
            for (int i = 0; i < filterList.Count; i++)
            {
                if (!filterList[i].IsCompleted)
                {
                    filterList.RemoveAt(i);
                }
            }
            Console.Write("Filtered Tasks: ");
            ShowTasks(filterList);
        }
        else if (userInput == "incomplete")
        {
            for (int i = 0; i < filterList.Count; i++)
            {
                if (filterList[i].IsCompleted)
                {
                    filterList.RemoveAt(i);
                }
            }
            Console.Write("Filtered Tasks: ");
            ShowTasks(filterList);
        }
        else
        {
            Console.WriteLine("Invalid input. Please try again.");
        }
        
    }
    
    /* Notes for FilterByStatus():
        - Current logic has bug(s). Filter is not being done correctly (e.g. incomplete tasks are being returned with complete tasks.
        - Find way to only have to loop through tasks once instead of twice
        - Find way to only show filtered task list until otherwise specified by user (Clear filter functionality)
    */
    
}