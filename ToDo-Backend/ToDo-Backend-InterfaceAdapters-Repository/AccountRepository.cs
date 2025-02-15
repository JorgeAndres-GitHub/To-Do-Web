using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ToDo_Backend_CA_AplicationLayer.Exceptions;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_InterfaceAdapters_Data;
using ToDo_Backend_FrameworksDrivers_API.Services.Common;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Mappers.Services;
using ToDo_Backend_InterfaceAdapters_Models;

namespace ToDo_Backend_InterfaceAdapters_Repository
{
    public class AccountRepository : IAccountRepository<UserEntity, AuthResult>
    {
        private readonly AppDbContext _context;
        private readonly ITaskRepository<TaskItem> _taskRepository;

        public AccountRepository(AppDbContext context, ITaskRepository<TaskItem> taskRepository)
        {
            _context = context;
            _taskRepository = taskRepository;
        }

        public async Task<AuthResult> CreateUserAsync(UserEntity user)
        {
            var cedulaEmailExist = await _context.Users.FirstOrDefaultAsync(x => x.IdentificationNumber == user.IdentificationNumber || x.Email == user.Email);
            
            if (cedulaEmailExist != null)
                throw new InvalidUserCreationException("User already exists.");

            var passwordHash = HashPassword.HashPasswordBD(user.Password); 

            var userModel = new UserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IdentificationNumber = user.IdentificationNumber,
                Country = user.Country,
                City = user.City,
                Phone = user.Phone,
                Email = user.Email,
                Password = passwordHash,
                CreatedTasks = user.CreatedTasks,
                CompletedTasks = user.CompletedTasks,
                PublishedTasks = user.PublishedTasks,
                IsEmailConfirmed = user.IsEmailConfirmed,
                VerificationCode = user.VerificationCode,
                UpdateConfirmationCode = user.UpdateConfirmationCode,
                IdRol = user.IdRol
            };            

            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();   
            
            return new AuthResult
            {
                User = await _context.Users.Where(u => u.IdentificationNumber.Equals(user.IdentificationNumber)).FirstAsync(),
                Result = true
            };

        }

        public async Task DeleteUserAsync(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var tasks = await _taskRepository.GetAllUserTasksAsync(id);

                    foreach (var task in tasks)
                    {
                        if (!task.IsPublic)
                            await _taskRepository.DeleteUserTaskAsync(task.Id, id);
                    }

                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                        throw new KeyNotFoundException("User not found.");
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();                  
                                       
                    await transaction.CommitAsync();                   
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<UserEntity> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return new UserEntity
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IdentificationNumber = user.IdentificationNumber,
                Country = user.Country,
                City = user.City,
                Phone = user.Phone,
                Email = user.Email,
                Password = user.Password,
                CreatedTasks = user.CreatedTasks,
                CompletedTasks = user.CompletedTasks,
                PublishedTasks = user.PublishedTasks,
                IsEmailConfirmed = user.IsEmailConfirmed,
                VerificationCode = user.VerificationCode,
                UpdateConfirmationCode = user.UpdateConfirmationCode,
                IdRol = user.IdRol
            };

            
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (existingUser == null)
                return new AuthResult{
                    Errors = new List<string> { "Invalid Payload" },
                    Result = false
                };

            if ((bool)!existingUser.IsEmailConfirmed)
                return new AuthResult
                {
                    Errors = new List<string> { "Email needs to be confirmed" },
                    Result = false
                };

            password = HashPassword.HashPasswordBD(password);

            var checkUserAndPass = existingUser.Password == password; 
            if(!checkUserAndPass)
                return new AuthResult
                {
                    Errors = new List<string> { "Invalid Credentials" },
                    Result = false
                };

            return new AuthResult
            {
                User = existingUser,
                Result = true
            };
        }

        public async Task UpdateUserAsync(UserEntity userEntity)
        {
            var userModel = await _context.Users.FindAsync(userEntity.Id);

            if(userEntity == null)
                throw new KeyNotFoundException("User not found.");

            userModel.FirstName = userEntity.FirstName;
            userModel.LastName = userEntity.LastName;
            userModel.IdentificationNumber = userEntity.IdentificationNumber;
            userModel.Country = userEntity.Country;
            userModel.City = userEntity.City;
            userModel.Phone = userEntity.Phone;
            userModel.Email = userEntity.Email;
            userModel.IdRol = userEntity.IdRol;
            
            await _context.SaveChangesAsync();
        }
    }
}

