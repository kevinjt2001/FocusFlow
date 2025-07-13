namespace FocusFlow.Console
{
    using System;
    internal class Program
    {
        public static void Main(string[] args)
        {
            TaskManager.Tasks = DataManager.LoadTasks();
            UI.Loop();
            DataManager.SaveTasks(TaskManager.Tasks);
            
        }
    }
}