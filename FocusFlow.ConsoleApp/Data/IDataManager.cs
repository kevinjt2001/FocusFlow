using FocusFlow.ConsoleApp.Models;

namespace FocusFlow.ConsoleApp.Data;

public interface IDataManager
{
    List<TaskItem> LoadTasks();
    void SaveTasks(List<TaskItem> tasks);
}