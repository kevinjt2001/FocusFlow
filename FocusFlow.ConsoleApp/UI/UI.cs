using FocusFlow.API.Data;
using FocusFlow.API.Services;

namespace FocusFlow.ConsoleApp.UI;

public class UI
{
    public static void PrintMenu()
    {
        Console.WriteLine("Task Manager");
        Console.WriteLine("  1. Add Task");
        Console.WriteLine("  2. Mark Task as Complete");
        Console.WriteLine("  3. Delete Task");
        Console.WriteLine("  4. Edit Task");
        Console.WriteLine("  5. Filter Tasks by Status");
        Console.WriteLine("  6. Clear Filter");
        Console.WriteLine("  7. Sort Tasks by Due Date");
        Console.WriteLine("  8. Clear Task Sort");
        Console.WriteLine("  9. Exit Task Manager");
        Console.Write("> ");
    }

    public static void Loop()
    {
        Console.WriteLine("Welcome to FocusFlow!");
        DataManager dataManager = new DataManager();
        TaskManager tm = new TaskManager(dataManager);

        while (true)
        {
            var tasks = tm.GetVisibleTaskDescriptions();
            Console.WriteLine("\n---------  Tasks  ---------");
            if (tasks.Count == 0)
                Console.WriteLine("No tasks to display.\n");
            else
                tasks.ForEach(Console.WriteLine);
                Console.WriteLine();

            PrintMenu();
            var userInput = Console.ReadLine()?.Trim();

            switch (userInput)
            {
                case "1":
                    AddTaskUI(tm);
                    Console.WriteLine();
                    break;
                case "2":
                    Console.Write("Enter task number to complete: ");
                    if (int.TryParse(Console.ReadLine(), out int cIndex))
                        Console.WriteLine(tm.CompleteTask(cIndex) ? "Task marked complete." : "Invalid selection.");
                    break;
                case "3":
                    Console.Write("Enter task number to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int dIndex))
                        Console.WriteLine(tm.DeleteTask(dIndex) ? "Task deleted." : "Invalid selection.");
                    break;
                case "4":
                    Console.Write("Enter task number to edit: ");
                    if (int.TryParse(Console.ReadLine(), out int eIndex))
                        EditTaskUI(tm, eIndex);
                    break;
                case "5":
                    Console.Write("Filter tasks by status (complete/incomplete): ");
                    var status = Console.ReadLine()?.Trim().ToLower();
                    Console.WriteLine(tm.FilterByStatus(status) ? $"Filter applied: {status}" : "Invalid filter.");
                    break;
                case "6":
                    tm.ClearFilter();
                    Console.WriteLine("Filter cleared.");
                    break;
                case "7":
                    Console.Write("Sort tasks by due date (oldest/newest): ");
                    var sort = Console.ReadLine()?.Trim().ToLower();
                    Console.WriteLine(tm.SortByDueDate(sort) ? $"Sorted by: {sort}" : "Invalid sort.");
                    break;
                case "8":
                    tm.ClearSort();
                    Console.WriteLine("Sort cleared.");
                    break;
                case "9":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
            }
        }
    }

    private static void AddTaskUI(TaskManager tm)
    {
        Console.Write("Enter task title: ");
        var title = Console.ReadLine();

        Console.Write("Enter task description (optional): ");
        var description = Console.ReadLine();

        Console.Write("Enter task due date (MM/dd/yyyy, optional): ");
        var dueDateInput = Console.ReadLine();
        var dueDate = TaskManager.ParseDueDate(dueDateInput);

        Console.Write("Enter task priority (low/medium/high): ");
        var priority = Console.ReadLine();

        if (!TaskManager.IsValidPriority(priority))
        {
            Console.WriteLine("Invalid priority.");
            return;
        }

        if (tm.AddTask(title, description, dueDate, priority))
            Console.WriteLine("Task added successfully.");
        else
            Console.WriteLine("Failed to add task.");
    }

    private static void EditTaskUI(TaskManager tm, int index)
    {
        Console.Write("Edit status? (y/n): ");
        var statusInput = Console.ReadLine()?.Trim().ToLower();
        bool? newStatus = null;
        if (statusInput == "y")
        {
            Console.Write("Enter new status (complete/incomplete): ");
            var status = Console.ReadLine()?.Trim().ToLower();
            newStatus = status == "complete";
        }

        Console.Write("Edit title? (y/n): ");
        var newTitle = Console.ReadLine()?.Trim().ToLower() == "y" ? Prompt("Enter new title: ") : null;

        Console.Write("Edit description? (y/n): ");
        var newDesc = Console.ReadLine()?.Trim().ToLower() == "y" ? Prompt("Enter new description: ") : null;

        Console.Write("Edit due date? (y/n): ");
        DateTime? newDate = null;
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            Console.Write("Enter new due date (MM/dd/yyyy): ");
            newDate = TaskManager.ParseDueDate(Console.ReadLine());
        }

        Console.Write("Edit priority? (y/n): ");
        var newPriority = Console.ReadLine()?.Trim().ToLower() == "y" ? Prompt("Enter new priority (low/medium/high): ") : null;

        var result = tm.EditTask(index, newTitle, newDesc, newDate, newStatus, newPriority);
        Console.WriteLine(result ? "Task updated." : "Failed to update task.");
    }

    private static string Prompt(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }
}
