namespace FocusFlow.ConsoleApp;

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
        TaskManager tm = new TaskManager();
        
        while (true)
        {
            tm.ShowTasks();
            PrintMenu();
            var userInput = Console.ReadLine()?.Trim();
            
            switch (userInput)
            {
                case "1":
                    tm.AddTask();
                    break;
                case "2":
                    tm.CompleteTask();
                    break;
                case "3":
                    tm.DeleteTask();
                    break;
                case "4":
                    tm.EditTask();
                    break;
                case "5":
                    tm.FilterByStatus();
                    break;
                case "6":
                    tm.ClearFilter();
                    break;
                case "7":
                    tm.SortByDueDate();
                    break;
                case "8":
                    tm.ClearSort();
                    break;
                case "9":
                    Console.Write("\nGoodbye!");
                    return; // exit 
                default:
                    Console.WriteLine("\nInvalid input. Please try again.");
                    break;
            }
            DataManager.SaveTasks(tm.Tasks);
        }
        
    }
}