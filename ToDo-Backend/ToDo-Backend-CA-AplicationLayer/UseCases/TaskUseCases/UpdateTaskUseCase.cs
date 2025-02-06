using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.TaskUseCases
{
    public class UpdateTaskUseCase<TDTO>
    {
        private readonly ITaskRepository<TaskItem> _repository;
        private readonly IMapper<TDTO, TaskItem> _mapper;

        public UpdateTaskUseCase(ITaskRepository<TaskItem> repository, IMapper<TDTO, TaskItem> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task ExecuteAsync(TDTO dto, int userId)
        {
            var taskItem = _mapper.ToEntity(dto);
            await _repository.UpdateTaskAsync(taskItem, userId);
        }
    }
}
