using Microsoft.EntityFrameworkCore;
using ToDo_Backend_CA_AplicationLayer.Exceptions;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_InterfaceAdapters_Data;
using ToDo_Backend_InterfaceAdapters_Models;

namespace ToDo_Backend_InterfaceAdapters_Repository
{
    public class TaskRepository : ITaskRepository<TaskItem>
    {
        private readonly AppDbContext _dbContext;

        public TaskRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<int> AddTaskAsync(TaskItem task, int userId)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var taskModel = new TaskModel
                    {
                        Title = task.Title,
                        Description = task.Description,
                        IsCompleted = task.IsCompleted,
                        CreatedAt = task.CreatedAt,
                        DueDate = task.DueDate
                    };
                    await _dbContext.Tasks.AddAsync(taskModel);
                    await _dbContext.SaveChangesAsync();

                    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user == null)
                        throw new KeyNotFoundException("No user found with the specified Id.");

                    var userTask = new UserTaskModel
                    {
                        TaskId = taskModel.Id,
                        UserId = userId
                    };

                    await _dbContext.UserTaskModels.AddAsync(userTask);
                    await _dbContext.SaveChangesAsync();

                    user.CreatedTasks += 1;
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return taskModel.Id;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<TaskItem>> GetAllUserTasksAsync(int userId)
        {
            var userTasks = await _dbContext.UserTaskModels.Where(ut => ut.UserId == userId).ToListAsync();

            if (!userTasks.Any())
                throw new KeyNotFoundException("No tasks found for the specified user.");

            var taskIds = userTasks.Select(ut => ut.TaskId).ToList();

            var tasks = await _dbContext.Tasks.Where(t => taskIds.Contains(t.Id)).ToListAsync();

            return tasks.Select(t => new TaskItem
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DueDate = t.DueDate,
                IsPublic = t.IsPublic
            }).ToList();
        }

        public async Task DeleteMultipleTasksAsync(IEnumerable<int> idList, int userId)
        {
            var userTasks = await GetAllUserTasksAsync(userId);

            var tasksToDelete = userTasks.Where(t => idList.Contains(t.Id)).ToList();            

            if (!tasksToDelete.Any())
                throw new KeyNotFoundException("No tasks found with the specified Ids.");

            var tasks = await _dbContext.Tasks
                .Where(t => idList.Contains(t.Id) && tasksToDelete.Any(td => td.Id == t.Id))
                .ToListAsync();

            if (!tasks.Any())
                throw new KeyNotFoundException("No matching tasks found in the database.");

            _dbContext.Tasks.RemoveRange(tasks);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserTaskAsync(int id, int userId)
        {
            var userTasks = await GetAllUserTasksAsync(userId);

            var task = userTasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                throw new TaskIdValidationException(id);

            var taskModel = await _dbContext.Tasks.FindAsync(task.Id);

            if(taskModel == null)
                throw new TaskIdValidationException(id);

            _dbContext.Tasks.Remove(taskModel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync() => await _dbContext.Tasks.Select(t => new TaskItem
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            DueDate = t.DueDate,
            IsPublic = t.IsPublic
        }).ToListAsync();

        public async Task<TaskItem> GetTaskAsync(int id, int userId)
        {
            var userTasks = await GetAllUserTasksAsync(userId);

            var task = userTasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                throw new TaskIdValidationException(id);

            var taskModel = await _dbContext.Tasks.FindAsync(task.Id);

            if (taskModel == null)
                throw new TaskIdValidationException(id);

            return new TaskItem
            {
                Id = taskModel.Id,
                Title = taskModel.Title,
                Description = taskModel.Description,
                IsCompleted = taskModel.IsCompleted,
                IsPublic = taskModel.IsPublic,
                CreatedAt = taskModel.CreatedAt,
                UpdatedAt = taskModel.UpdatedAt,
                DueDate = taskModel.DueDate 
            };
        }

        public async Task MarkAsCompletedAsync(int id, int userId)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var userTasks = await GetAllUserTasksAsync(userId);

                    if (!userTasks.Any())
                        throw new KeyNotFoundException("No tasks found for the specified user.");

                    var task = userTasks.FirstOrDefault(t => t.Id == id);

                    if (task == null)
                        throw new TaskIdValidationException(id);

                    var taskModel = await _dbContext.Tasks.FindAsync(task.Id);

                    if (taskModel == null)
                        throw new TaskIdValidationException(id);

                    var taskItem = new TaskItem();

                    taskItem.MarkAsCompleted();

                    taskModel.IsCompleted = taskItem.IsCompleted;
                    taskModel.UpdatedAt = DateTime.Now;

                    await _dbContext.SaveChangesAsync();

                    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null)
                        throw new KeyNotFoundException("No user found with the specified Id.");

                    user.CompletedTasks += 1;

                    if (user.CompletedTasks == 2)
                    {
                        user.IdRol = 2;
                    }

                    if (user.CompletedTasks == 4)
                    {
                        user.IdRol = 1;
                    }

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task UpdateTaskAsync(TaskItem taskItem, int userId)
        {
            var userTasks = await GetAllUserTasksAsync(userId);
            if (userTasks == null)
                throw new KeyNotFoundException("No tasks found for the specified user.");

            var task = userTasks.FirstOrDefault(t => t.Id == taskItem.Id);

            if (task == null)
                throw new TaskIdValidationException(taskItem.Id);

            var taskModel = await _dbContext.Tasks.FindAsync(task.Id);

            if (taskModel == null)
                throw new TaskIdValidationException(taskItem.Id);

            taskModel.Title = string.IsNullOrEmpty(taskItem.Title) ? taskModel.Title : taskItem.Title;
            taskModel.Description = string.IsNullOrEmpty(taskItem.Description) ? taskModel.Description : taskItem.Description;
            taskModel.UpdatedAt = DateTime.Now;
            taskModel.DueDate = taskItem.DueDate == null ? taskModel.DueDate : taskItem.DueDate;

            await _dbContext.SaveChangesAsync();
        }
    }
}
