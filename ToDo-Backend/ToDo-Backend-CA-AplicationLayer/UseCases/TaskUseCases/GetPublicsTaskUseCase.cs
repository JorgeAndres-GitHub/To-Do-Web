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
    public class GetPublicsTaskUseCase<TOutput>
    {
        private readonly ITaskRepository<TaskItem> _taskRepository;
        private readonly IPresenter<TaskItem, TOutput> _presenter;

        public GetPublicsTaskUseCase(ITaskRepository<TaskItem> taskRepository, IPresenter<TaskItem, TOutput> presenter)
        {
            _taskRepository = taskRepository;
            _presenter = presenter;
        }

        public async Task<IEnumerable<TOutput>> ExecuteAsync()
        {
            var tasks = await _taskRepository.GetPublicsTasksAsync();
            return _presenter.Present(tasks);
        }

    }
}
