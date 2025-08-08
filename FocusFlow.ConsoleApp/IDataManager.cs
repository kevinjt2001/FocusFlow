namespace FocusFlow.ConsoleApp;

public interface IDataManager
{
    List<TaskItem> LoadTasks();
    void SaveTasks(List<TaskItem> tasks);
}