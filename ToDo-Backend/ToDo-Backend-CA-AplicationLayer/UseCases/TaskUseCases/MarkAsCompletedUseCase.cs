using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.TaskUseCases
{
    public class MarkAsCompletedUseCase
    {
        private readonly ITaskRepository<TaskItem> _repository;

        public MarkAsCompletedUseCase(ITaskRepository<TaskItem> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(int id, int userId)
        {
            await _repository.MarkAsCompletedAsync(id, userId);
        }
    }
}
