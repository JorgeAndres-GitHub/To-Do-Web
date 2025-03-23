using Microsoft.EntityFrameworkCore;
using Moq;
using NSubstitute;
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
        public async Task CreateUser_ShouldReturnTrue_WhenUserOk()
        {
            // Arrange 
            var user = new UserEntity
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

            // Act
            var response = await _accountRepository.CreateUserAsync(user);

            // Assert
            Assert.True(response.Result);
            Assert.Contains(response.User.IdentificationNumber, user.IdentificationNumber);            
        }

        [Fact]
        public async Task Login_ShouldReturnTrue_WhenTrueCredentials()
        {
            // Arrange
            var user = new UserEntity
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

            var userConfirmed = new UserEntity
            {
                Id = 2,
                FirstName = "Coavas",
                LastName = "Doe",
                IdentificationNumber = "1354151435",
                Country = "Ecuador",
                City = "Quito",
                Phone = "0987654321",
                Email = "coavas@example.com",
                Password = "123456",
                CreatedTasks = 0,
                CompletedTasks = 0,
                PublishedTasks = 0,
                IsEmailConfirmed = true,
                VerificationCode = null,
                UpdateConfirmationCode = null,
                IdRol = 1
            };

            // Act
            var userResponse = await _accountRepository.CreateUserAsync(user);
            var authentication = await _accountRepository.LoginAsync(userResponse.User.Email, user.Password);
            var authenticationIssue = await _accountRepository.LoginAsync("johndoeh@example.com", userResponse.User.Password);

            var userResponseConfirmed = await _accountRepository.CreateUserAsync(userConfirmed);
            var authenticationConfirmed = await _accountRepository.LoginAsync(userResponseConfirmed.User.Email, userConfirmed.Password);
            var authenticationIssueConfirmed = await _accountRepository.LoginAsync("coavash@example.com", userResponseConfirmed.User.Password);

            // Assert
            Assert.True(!authentication.Result);
            Assert.Equal(authentication.Errors, new List<string> { "Email needs to be confirmed" });
            Assert.True(!authenticationIssue.Result);

            Assert.Null(authenticationConfirmed.Errors);
            Assert.True(authenticationConfirmed.Result);
            Assert.True(!authenticationIssueConfirmed.Result);

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


