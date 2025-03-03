using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.TaskUseCases
{
    public class AssignPublicTaskUseCase
    {
        private readonly ITaskRepository<TaskItem> _taskRepository;

        public AssignPublicTaskUseCase(ITaskRepository<TaskItem> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task ExecuteAsync(int userId, int taskId) =>
            await _taskRepository.AssignPublicTaskAsync(userId, taskId);
        
    }
}
