using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces
{
    public interface ITaskRepository<T>
    {
        Task<IEnumerable<T>> GetAllUserTasksAsync(int userId);
        Task<T> GetTaskAsync(int id, int userId);
        Task<IEnumerable<T>> GetAllTasksAsync();
        Task<(int taskId, bool shouldRefreshToken)> AddTaskAsync(T task, int userId);
        Task DeleteUserTaskAsync(int id, int userId);
        Task UpdateTaskAsync(T task, int userId);
        Task DeleteMultipleTasksAsync(IEnumerable<int> idList, int userId);
        Task<bool> MarkAsCompletedAsync(int id, int userId);
        Task PostTaskAsync(int taskId);
        Task<IEnumerable<T>> GetPublicsTasksAsync();
    }
}
