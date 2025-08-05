using System.Globalization;

namespace FocusFlow;
using System;

public class TaskManager
{
    public List<TaskItem> Tasks { get; set; }
    public List<TaskItem> VisibleTasks { get; set; }
    private string? CurrentFilter = null;
    private string? SortOrder = null;

    public TaskManager()
    {
        Tasks = DataManager.LoadTasks();
        VisibleTasks = new List<TaskItem>(Tasks);
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
    
    public void ClearFilter()
    {
        CurrentFilter = null;
        Console.WriteLine("Task filter cleared. Showing all tasks.");
    }

    public void ClearSort()
    {
        SortOrder = null;
        Console.WriteLine("Task sort cleared. Showing all tasks.");
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

            // Get user input for task priority and validate it. Priority cannot be null. 
            string priority = ValidatePriority();
            task.Priority = priority;
            
            Tasks.Add(task);
        }
        DataManager.SaveTasks(Tasks);
    }

    public void ShowTasks()
    {
        // Apply filter first
        var filteredTasks = string.IsNullOrEmpty(CurrentFilter)
            ? Tasks
            : Tasks.Where(t => CurrentFilter == "complete" ? t.IsCompleted : !t.IsCompleted).ToList();
        
        // Apply sort
        if (!string.IsNullOrEmpty(SortOrder))
        {
            filteredTasks = SortOrder == "oldest"
                ? filteredTasks.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ToList()
                : filteredTasks.OrderByDescending(t => t.DueDate ?? DateTime.MinValue).ToList();
        }

        VisibleTasks = filteredTasks;
        
        
        // Task display
        Console.WriteLine("\n---------  Tasks  ---------");
        if (!filteredTasks.Any())
        {
            Console.WriteLine("No tasks to display.\n");
            return;
        }

        if (!string.IsNullOrEmpty(CurrentFilter))
            Console.WriteLine($"(Filter applied: Showing only {CurrentFilter} tasks.)");
        

        if (!string.IsNullOrEmpty(SortOrder))
            Console.WriteLine($"(Tasks sorted by due date: {SortOrder})");
        
        
        // Loop through tasks and display them
        for (int i = 0; i < filteredTasks.Count; i++)
        {
            var status = filteredTasks[i].IsCompleted ? "[\u2713] Complete  " : "[ ] Incomplete";
            var dueDateDisplay = filteredTasks[i].DueDate?.ToString("MM/dd/yyyy") ?? "No due date";
            var priority = filteredTasks[i].Priority ?? "No priority";
            Console.WriteLine($"{i + 1}. {status} | {filteredTasks[i].Title} - {filteredTasks[i].Description} - (Due: {dueDateDisplay}) - (Priority: {priority})");
        }
        
        Console.WriteLine();
    }

    public void CompleteTask()
    {
        if (!CheckForTasks("No tasks to complete.")) return;
            
        // Mark task complete by task number (index)
        Console.Write("Enter task number to complete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= VisibleTasks.Count)
        {
            var selectedTask = VisibleTasks[index - 1];
            if (selectedTask.IsCompleted)
            {
                Console.WriteLine("\nTask is already complete.");
                return;
            }
            selectedTask.IsCompleted = true;
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
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= VisibleTasks.Count)
        {
            var deletedTask = VisibleTasks[index - 1];
            if (CheckYesOrNo($"Are you sure you want to delete this task, ({deletedTask.Title})? (y/n): "))
            {
                Tasks.Remove(deletedTask);
                Console.WriteLine($"Task deleted: {deletedTask.Title}");
                // Update tasks.json
                DataManager.SaveTasks(Tasks);
            }
        }
        else
        {
            Console.WriteLine("\nInvalid task number. Please try again.");
        }
    }
    
    // Method to validate DueDate
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

    public string ValidatePriority()
    {
        while (true)
        {
            Console.Write("Enter task priority (low/medium/high): ");
            string priority = Console.ReadLine().Trim().ToLower();
            
            if (priority == "low" || priority == "medium" || priority == "high")
            {
                return priority;
            }
            
            if (string.IsNullOrWhiteSpace(priority))
            {
                Console.WriteLine("\nTask priority may not be empty. Please try again (low/medium/high).");
            }

            else
            {
                Console.WriteLine("\nInvalid task priority. Please try again (low/medium/high).");
            }
        }
        
    }
    
    public void EditTask()
    {
        if (!CheckForTasks("No tasks to edit.")) return;
        
        Console.Write("Enter task number to edit: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= VisibleTasks.Count)
        {
            var task = VisibleTasks[index - 1];

            var taskStatus = task.IsCompleted ? "[\u2713] Complete  " : "[ ] Incomplete";
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
            
            Console.WriteLine($"\nCurrent priority: {task.Priority}");
            if (CheckYesOrNo("Would you like to edit this priority? (y/n): "))
            {
                string priority = ValidatePriority();
                task.Priority = priority;
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
        
        Console.Write("Filter tasks by status (complete/incomplete): ");
        string status = Console.ReadLine().Trim().ToLower();
        
        if (status == "complete" || status == "incomplete")
        {
            CurrentFilter = status;
            return;
        }
        
        Console.WriteLine("Invalid filter input. Please enter 'complete' or 'incomplete' to filter tasks by status.");
    }

    public void SortByDueDate()
    {
        if (!CheckForTasks("No tasks to sort.")) return;
        
        Console.Write("Sort tasks by due date (oldest/newest): ");
        string sort = Console.ReadLine().Trim().ToLower();

        if (sort == "oldest" || sort == "newest")
        {
            SortOrder = sort;
            return;
        }
            
        Console.WriteLine("Invalid sort input. Please enter 'oldest' or 'newest' to sort tasks by due date.");
    }
}
