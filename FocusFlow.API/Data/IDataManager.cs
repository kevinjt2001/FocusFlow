using FocusFlow.API.Models;

namespace FocusFlow.API.Data;

public interface IDataManager
{
    List<TaskItem> LoadTasks();
    void SaveTasks(List<TaskItem> tasks);
}