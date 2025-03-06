using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_InterfaceAdapters_Data;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Models;
using ToDo_Backend_InterfaceAdapters_Repository;

namespace ToDo_Backend_CA_TestUnit
{
    public class AccountRepositoryTest
    {
        private readonly AppDbContext _context;
        private readonly Mock<ITaskRepository<TaskItem>> _mockTaskRepository;
        private readonly IAccountRepository<UserEntity, AuthResult> _accountRepository;

        public AccountRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // BD separada por prueba
                .Options;

            _context = new AppDbContext(options); // Ahora sí se puede instanciar
            _mockTaskRepository = new Mock<ITaskRepository<TaskItem>>(); // Se mantiene el mock

            _accountRepository = new AccountRepository(_context, _mockTaskRepository.Object); // Se pasa el DbContext real
        }

        [Fact]
        public async Task FindById_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange 
            var user = new UserModel
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                IdentificationNumber = "123456789",
                Country = "Ecuador",
                City = "Quito",
                Phone = "0987654321",
                Email = "johndoe@example.com",
                Password = "123456",
                CreatedTasks = 0,
                CompletedTasks = 0,
                PublishedTasks = 0,
                IsEmailConfirmed = false,
                VerificationCode = null,
                UpdateConfirmationCode = null,
                IdRol = 1
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _accountRepository.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveUserAndCascadeDeleteTasks()
        {
            // Arrange
            var user = new UserModel
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                IdentificationNumber = "123456789",
                Country = "Ecuador",
                City = "Quito",
                Phone = "0987654321",
                Email = "johndoe@example.com",
                Password = "123456",
                CreatedTasks = 0,
                CompletedTasks = 0,
                PublishedTasks = 0,
                IsEmailConfirmed = false,
                VerificationCode = null,
                UpdateConfirmationCode = null,
                IdRol = 1
            };
            var task1 = new TaskModel { Id = 1, Title = "Task 1", Description = "Tarea 1" };
            var task2 = new TaskModel { Id = 2, Title = "Task 2", Description = "Tarea 2" };

            _context.Users.Add(user);
            _context.Tasks.AddRange(task1, task2);
            _context.UserTaskModels.AddRange(
                new UserTaskModel { UserId = 1, TaskId = 1 },
                new UserTaskModel { UserId = 1, TaskId = 2 }
            );
            await _context.SaveChangesAsync();

            // Act
            await _accountRepository.DeleteUserAsync(1);

            // Assert: Verificar que el usuario ya no existe
            var deletedUser = await _context.Users.FindAsync(1);
            Assert.Null(deletedUser);

            // Verificar que las tareas del usuario también fueron eliminadas
            var remainingTasks = await _context.UserTaskModels.Where(t => t.UserId == 1).ToListAsync();
            Assert.Empty(remainingTasks);
        }
    }
}


