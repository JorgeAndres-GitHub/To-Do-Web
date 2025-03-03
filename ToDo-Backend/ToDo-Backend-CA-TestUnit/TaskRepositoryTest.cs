using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_AplicationLayer.UseCases.TaskUseCases;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Views;
using ToDo_Backend_CA_InterfaceAdapters_Data;
using ToDo_Backend_FrameworksDrivers_API.Controllers;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.UserRequests;
using ToDo_Backend_InterfaceAdapters_Models;
using ToDo_Backend_InterfaceAdapters_Repository;

namespace ToDo_Backend_CA_TestUnit
{
    public class TaskRepositoryTest
    {
        private readonly AppDbContext _dbContext;
        private readonly TaskRepository _taskRepository;

        public TaskRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = new AppDbContext(options);
            _taskRepository = new TaskRepository(_dbContext);
        }

        [Fact]
        public async Task AddTaskAsync_ShouldAddTask()
        {
            // Arrange
            var taskItem = new TaskItem { Title = "Test Task", Description = "Description", IsCompleted = false };
            var user1 = new UserModel
            {
                Id = 1,
                CreatedTasks = 2,
                IdRol = 3,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "#SecurePassword123",
                Phone = "123456789",
                IdentificationNumber = "ABC123454255",
                City = "Cartagena",
                Country = "Colombia"
            };

            var user2 = new UserModel
            {
                Id = 2,
                CreatedTasks = 3,
                IdRol = 2,
                FirstName = "Jorge",
                LastName = "Herrera",
                Email = "jorgeahm@example.com",
                Password = "#SecurePassword123",
                Phone = "123456789",
                IdentificationNumber = "ABC123454255",
                City = "Cartagena",
                Country = "Colombia"
            };

            _dbContext.Users.Add(user1);
            _dbContext.Users.Add(user2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result1 = await _taskRepository.AddTaskAsync(taskItem, 1);
            var result2 = await _taskRepository.AddTaskAsync(taskItem, 2);

            // Assert
            Assert.Equal(1, result1.taskId);
            Assert.True(result1.shouldRefreshToken);

            Assert.Equal(2, result2.taskId);
            Assert.False(result2.shouldRefreshToken);
        }

        [Fact]
        public async Task GetAllUserTasksAsync_ShouldReturnUserTasks()
        {
            // Arrange
            var userId1 = 1;
            var userId2 = 2;

            var tasks1 = new List<TaskModel>
            {
                new TaskModel { Id = 1, Title = "Task 1", Description = "Desc 1", IsCompleted = false },
                new TaskModel { Id = 2, Title = "Task 2", Description = "Desc 2", IsCompleted = true }
            };

            var tasks2 = new List<TaskModel>
            {
                new TaskModel { Id = 3, Title = "Task 3", Description = "Desc task 3", IsCompleted = false },
                new TaskModel { Id = 4, Title = "Task 4", Description = "Desc task 4", IsCompleted = true }
            };

            var userTasks = new List<UserTaskModel>
            {
                new UserTaskModel { UserId = userId1, TaskId = 1 },
                new UserTaskModel { UserId = userId1, TaskId = 2 },
                new UserTaskModel { UserId = userId2, TaskId = 3 },
                new UserTaskModel { UserId = userId2, TaskId = 4 }
            };

            _dbContext.Tasks.AddRange(tasks1);
            _dbContext.Tasks.AddRange(tasks2);
            _dbContext.UserTaskModels.AddRange(userTasks);
            await _dbContext.SaveChangesAsync();

            // Act
            var result1 = await _taskRepository.GetAllUserTasksAsync(userId1);
            var result2 = await _taskRepository.GetAllUserTasksAsync(userId2);

            // Assert
            Assert.NotNull(result1);
            Assert.Equal(2, result1.Count());
            Assert.Contains(result1, t => t.Id == 1 && t.Title == "Task 1" && t.Description == "Desc 1");
            Assert.Contains(result1, t => t.Id == 2 && t.Title == "Task 2" && t.Description == "Desc 2");

            Assert.NotNull(result2);
            Assert.Equal(2, result2.Count());
            Assert.Contains(result2, t => t.Id == 3 && t.Title == "Task 3" && t.Description == "Desc task 3");
            Assert.Contains(result2, t => t.Id == 4 && t.Title == "Task 4" && t.Description == "Desc task 4");
        }

        [Fact]
        public async Task DeleteMultipleTasksAsync_ShouldRemoveSpecifiedTasks()
        {
            // Arrange
            var userId = 1;
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, Title = "Task 1", Description = "Desc 1", IsCompleted = false },
                new TaskModel { Id = 2, Title = "Task 2", Description = "Desc 2", IsCompleted = true },
                new TaskModel { Id = 3, Title = "Task 3", Description = "Desc 3", IsCompleted = false },
                new TaskModel { Id = 4, Title = "Task 4", Description = "Desc 4", IsCompleted = true }
            };
            var userTasks = new List<UserTaskModel>
            {
                new UserTaskModel { UserId = userId, TaskId = 1 },
                new UserTaskModel { UserId = userId, TaskId = 2 },
                new UserTaskModel { UserId = userId, TaskId = 3 },
                new UserTaskModel { UserId = userId, TaskId = 4 }
            };
            _dbContext.Tasks.AddRange(tasks);
            _dbContext.UserTaskModels.AddRange(userTasks);
            await _dbContext.SaveChangesAsync();
            // Act
            await _taskRepository.DeleteMultipleTasksAsync(new List<int> { 1, 3 }, userId);
            var deletedTask1 = await _dbContext.Tasks.FindAsync(1);
            var undeletedTask1 = await _dbContext.Tasks.FindAsync(2);
            var deletedTask2 = await _dbContext.Tasks.FindAsync(3);
            var undeletedTask2 = await _dbContext.Tasks.FindAsync(4);
            // Assert
            Assert.Null(deletedTask1);
            Assert.NotNull(undeletedTask1);
            Assert.Null(deletedTask2);
            Assert.NotNull(undeletedTask2);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldDeleteTask()
        {
            // Arrange
            var userId1 = 1;

            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, Title = "Task 1", Description = "Desc 1", IsCompleted = false },
                new TaskModel { Id = 2, Title = "Task 2", Description = "Desc 2", IsCompleted = true }
            };

            var userTasks = new List<UserTaskModel>
            {
                new UserTaskModel { UserId = userId1, TaskId = 1 },
                new UserTaskModel { UserId = userId1, TaskId = 2 }
            };

            _dbContext.Tasks.AddRange(tasks);
            _dbContext.UserTaskModels.AddRange(userTasks);
            await _dbContext.SaveChangesAsync();

            // Act

            await _taskRepository.DeleteUserTaskAsync(1, userId1);

            TaskModel? deletedTask = await _dbContext.Tasks.FindAsync(1);
            TaskModel undeletedTask = await _dbContext.Tasks.FindAsync(2);

            // Assert
            Assert.Null(deletedTask);
            Assert.NotNull(undeletedTask);
        }

        [Fact]
        public async Task GetTaskAsync_ShouldReturnUserTask()
        {
            // Arrange
            var userId = 1;
            var task1 = new TaskModel { Id = 1, Title = "Task 1", Description = "Desc 1", IsCompleted = false };
            var task2 = new TaskModel { Id = 2, Title = "Task 2", Description = "Desc 2", IsCompleted = true };
            var tasks = new List<TaskModel>
            {
                task1,
                task2
            };

            var userTasks = new List<UserTaskModel>
            {
                new UserTaskModel { UserId = userId, TaskId = 1 },
                new UserTaskModel { UserId = userId, TaskId = 2 }
            };

            _dbContext.Tasks.AddRange(tasks);
            _dbContext.UserTaskModels.AddRange(userTasks);
            await _dbContext.SaveChangesAsync();

            // Act 
            var result1 = await _taskRepository.GetTaskAsync(1, userId);
            var result2 = await _taskRepository.GetTaskAsync(2, userId);

            // Assert
            Assert.Equal(result1.Description, task1.Description);
            Assert.Equal(result2.Description, task2.Description);

        }

        [Fact]
        public async Task MarkAsCompletedAsync_ShouldMarkTaskAsCompleted()
        {
            // Arrange
            var userId = 1;
            var taskId = 1;

            var user = new UserModel
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "#SecurePassword123",
                Phone = "123456789",
                IdentificationNumber = "ABC123454255",
                City = "Cartagena",
                Country = "Colombia",
                CompletedTasks = 4,
                IdRol = 3
            };

            var task = new TaskModel
            {
                Id = taskId,
                Title = "Sample Task",
                Description = "A task to complete",
                IsCompleted = false,
                UpdatedAt = DateTime.Now
            };

            var userTask = new UserTaskModel { UserId = userId, TaskId = taskId };

            _dbContext.Users.Add(user);
            _dbContext.Tasks.Add(task);
            _dbContext.UserTaskModels.Add(userTask);
            await _dbContext.SaveChangesAsync();

            // Act
            await _taskRepository.MarkAsCompletedAsync(taskId, userId);

            var updatedTask = await _dbContext.Tasks.FindAsync(taskId);
            var updatedUser = await _dbContext.Users.FindAsync(userId);

            // Assert
            Assert.NotNull(updatedTask);
            Assert.True(updatedTask.IsCompleted);
            Assert.Equal(5, updatedUser.CompletedTasks);
            Assert.Equal(1, updatedUser.IdRol); // Verifica si el rol cambió correctamente
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldUpdateTaskDetails()
        {
            // Arrange
            var userId = 1;
            var taskId = 1;
            var user = new UserModel
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "#SecurePassword123",
                Phone = "123456789",
                IdentificationNumber = "ABC123454255",
                City = "Cartagena",
                Country = "Colombia",
                CompletedTasks = 4,
                IdRol = 3
            };

            var task = new TaskModel
            {
                Id = taskId,
                Title = "Old Title",
                Description = "Old Description",
                IsCompleted = false,
                UpdatedAt = DateTime.Now.AddDays(-1),
                DueDate = DateTime.Now.AddDays(5)
            };

            var taskUpdatedAt = task.UpdatedAt;

            var updatedTaskItem = new TaskItem
            {
                Id = taskId,
                Title = "New Title",
                Description = "New Description",
                DueDate = DateTime.Now.AddDays(10)
            };

            var userTask = new UserTaskModel { UserId = userId, TaskId = taskId };

            _dbContext.Users.Add(user);
            _dbContext.Tasks.Add(task);
            _dbContext.UserTaskModels.Add(userTask);
            await _dbContext.SaveChangesAsync();

            // Act
            await _taskRepository.UpdateTaskAsync(updatedTaskItem, userId);

            var updatedTask = await _dbContext.Tasks.FindAsync(taskId);

            // Assert
            Assert.NotNull(updatedTask);
            Assert.Equal(updatedTaskItem.Title, updatedTask.Title);
            Assert.Equal(updatedTaskItem.Description, updatedTask.Description);
            Assert.Equal(updatedTaskItem.DueDate, updatedTask.DueDate);
            Assert.True(updatedTask.UpdatedAt > taskUpdatedAt);
        }

        [Fact]
        public async Task PostTaskAsync_ShouldSetTaskAsPublic()
        {
            // Arrange
            var taskId = 1;

            var task = new TaskModel
            {
                Id = taskId,
                Title = "Public Task",
                Description = "Old Description",
                IsCompleted = false,
                IsPublic = false
            };

            await _dbContext.Tasks.AddAsync(task);
            await _dbContext.SaveChangesAsync();

            // Act
            await _taskRepository.PostTaskAsync(taskId);
            var updatedTask = await _dbContext.Tasks.FindAsync(taskId);

            // Assert
            Assert.NotNull(updatedTask);
            Assert.True(updatedTask.IsPublic);
        }


        [Fact]
        public async Task GetPublicsTasksAsync_ShouldReturnPublicsTasks()
        {
            // Arrange
            var task1 = new TaskModel
            {
                Id = 1,
                Title = "Public Task",
                Description = "Test Task",
                IsCompleted = false,
                IsPublic = true
            };
            var task2 = new TaskModel
            {
                Id = 2,
                Title = "Private Task",
                Description = "Test Task",
                IsCompleted = false,
                IsPublic = false
            };

            var tasksList = new List<TaskModel> { task1, task2 };

            await _dbContext.Tasks.AddRangeAsync(tasksList);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskRepository.GetPublicsTasksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.FirstOrDefault(t => t.Title == task1.Title) != null);

        }

        [Fact]
        public async Task AssignPublicTaskAsync_ShouldAssignTaskToUser()
        {
            // Arrange
            var userId = 1;
            var taskId = 1;
            var user = new UserModel
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "#SecurePassword123",
                Phone = "123456789",
                IdentificationNumber = "ABC123454255",
                City = "Cartagena",
                Country = "Colombia",
                CompletedTasks = 4,
                IdRol = 3
            };

            var task = new TaskModel
            {
                Id = taskId,
                Title = "Sample Task",
                Description = "A task to complete",
                IsCompleted = false,
                UpdatedAt = DateTime.Now,
                IsPublic = true
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.Tasks.AddAsync(task);
            await _dbContext.SaveChangesAsync();

            // Act
            await _taskRepository.AssignPublicTaskAsync(userId, taskId);

            var userTaskModel = await _dbContext.UserTaskModels.FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TaskId == taskId);

            // Assert
            Assert.NotNull(userTaskModel);
        }
    }
}