namespace FocusFlow.Console
{
    using System;
    internal class Program
    {
        public static void Main(string[] args)
        {
            TaskItem.Tasks = DataManager.LoadTasks();
            UI.Loop();
            DataManager.SaveTasks(TaskItem.Tasks);
            
        }
    }
}