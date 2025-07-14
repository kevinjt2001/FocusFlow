namespace FocusFlow.Console;
using System;  
public class UI
{
    public static void PrintMenu()
    {
        Console.WriteLine("Task Manager");
        Console.WriteLine("  1. Add Task");
        Console.WriteLine("  2. Mark Task as Complete");
        Console.WriteLine("  3. Delete Task");
        Console.WriteLine("  4. Exit Task Manager");
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