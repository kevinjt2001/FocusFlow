using FocusFlow.ConsoleApp;
using FocusFlow.ConsoleApp.Data;
using FocusFlow.ConsoleApp.Models;
using FocusFlow.ConsoleApp.Services;
using Moq; 

namespace FocusFlow.Tests
{
    public class TaskManagerTests
    {
        private readonly Mock<IDataManager> _mockDataManager;
        private readonly TaskManager _taskManager;
        private readonly List<TaskItem> _sampleTasks;

        public TaskManagerTests()
        {
            // Arrange: Setup mock data and dependency
            _sampleTasks = new List<TaskItem>
            {
                new TaskItem { Title = "Task 1", Description = "Desc 1", IsCompleted = false, Priority = "low" },
                new TaskItem { Title = "Task 2", Description = "Desc 2", IsCompleted = true, Priority = "high" }
            };

            _mockDataManager = new Mock<IDataManager>();

            // Mock LoadTasks to always return our sample list
            _mockDataManager.Setup(dm => dm.LoadTasks()).Returns(_sampleTasks);

            // Mock SaveTasks so it does nothing (we just verify it gets called)
            _mockDataManager.Setup(dm => dm.SaveTasks(It.IsAny<List<TaskItem>>()));

            // Create TaskManager with mocked IDataManager
            _taskManager = new TaskManager(_mockDataManager.Object);
        }

        [Fact]
        public void AddTask_ValidData_ShouldAddTask()
        {
            // Act
            bool result = _taskManager.AddTask("New Task", "Description", DateTime.Now, "low");

            // Assert
            Assert.True(result);
            Assert.Contains(_taskManager.Tasks, t => t.Title == "New Task");
            _mockDataManager.Verify(dm => dm.SaveTasks(It.IsAny<List<TaskItem>>()), Times.Once);
        }

        [Fact]
        public void AddTask_InvalidData_ShouldReturnFalse()
        {
            bool result = _taskManager.AddTask("", "Description", DateTime.Now, "low");
            Assert.False(result);
        }

        [Fact]
        public void CompleteTask_ValidIndex_ShouldMarkComplete()
        {
            // Act
            bool result = _taskManager.CompleteTask(1);

            // Assert
            Assert.True(result);
            Assert.True(_taskManager.Tasks[0].IsCompleted);
            _mockDataManager.Verify(dm => dm.SaveTasks(It.IsAny<List<TaskItem>>()), Times.Once);
        }

        [Fact]
        public void CompleteTask_InvalidIndex_ShouldReturnFalse()
        {
            bool result = _taskManager.CompleteTask(99);
            Assert.False(result);
        }

        [Fact]
        public void DeleteTask_ValidIndex_ShouldRemoveTask()
        {
            bool result = _taskManager.DeleteTask(1);
            Assert.True(result);
            Assert.DoesNotContain(_taskManager.Tasks, t => t.Title == "Task 1");
        }

        [Fact]
        public void EditTask_ValidIndex_ShouldUpdateTask()
        {
            bool result = _taskManager.EditTask(1, newTitle: "Updated Title");
            Assert.True(result);
            Assert.Equal("Updated Title", _taskManager.Tasks[0].Title);
        }

        [Fact]
        public void FilterByStatus_Valid_ShouldSetFilter()
        {
            bool result = _taskManager.FilterByStatus("complete");
            Assert.True(result);
        }

        [Fact]
        public void SortByDueDate_Valid_ShouldSetSortOrder()
        {
            bool result = _taskManager.SortByDueDate("oldest");
            Assert.True(result);
        }

        [Fact]
        public void ParseDueDate_Valid_ShouldReturnDate()
        {
            DateTime? date = TaskManager.ParseDueDate("08/07/2025");
            Assert.NotNull(date);
            Assert.Equal(2025, date.Value.Year);
        }

        [Fact]
        public void IsValidPriority_ValidValues_ShouldReturnTrue()
        {
            Assert.True(TaskManager.IsValidPriority("low"));
            Assert.True(TaskManager.IsValidPriority("medium"));
            Assert.True(TaskManager.IsValidPriority("high"));
        }

        [Fact]
        public void IsValidPriority_InvalidValue_ShouldReturnFalse()
        {
            Assert.False(TaskManager.IsValidPriority("urgent"));
        }

        [Fact]
        public void GetVisibleTaskDescriptions_ShouldReturnFormattedList()
        {
            var descriptions = _taskManager.GetVisibleTaskDescriptions();
            Assert.NotEmpty(descriptions);
            Assert.Contains(descriptions, d => d.Contains("Task 1"));
        }
    }
}
