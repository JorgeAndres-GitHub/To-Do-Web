using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
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
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: "Test").Options;
            _dbContext = new AppDbContext(options);
            _taskRepository = new TaskRepository(_dbContext);
        }

        [Fact]
        public async Task AddTaskAsync_ShouldAddTask()
        {
            // Arrange
            var taskItem = new TaskItem { Title = "Test Task", Description = "Description", IsCompleted = false };
            var user = new UserModel
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

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskRepository.AddTaskAsync(taskItem, 1);

            // Assert
            Assert.Equal(1, result.taskId);
            Assert.True(result.shouldRefreshToken);
        }

        [Fact]
        public async Task GetAllUserTasksAsync_ShouldReturnUserTasks()
        {
            // Arrange
            var userId = 1;

            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, Title = "Task 1", Description = "Desc 1", IsCompleted = false },
                new TaskModel { Id = 2, Title = "Task 2", Description = "Desc 2", IsCompleted = true }
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
            var result = await _taskRepository.GetAllUserTasksAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Id == 1 && t.Title == "Task 1");
            Assert.Contains(result, t => t.Id == 2 && t.Title == "Task 2");
        }


    }
}
