using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.TaskUseCases
{
    public class DeleteMultipleTasksUseCase
    {
        private readonly ITaskRepository<TaskItem> _repository;

        public DeleteMultipleTasksUseCase(ITaskRepository<TaskItem> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(IEnumerable<int> idList, int userId)
        {
            await _repository.DeleteMultipleTasksAsync(idList, userId);
        }
    }
}
