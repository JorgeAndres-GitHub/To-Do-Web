using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.TaskUseCases
{
    public class GetTaskUseCase<T>
    {
        private readonly ITaskRepository<T> _repository;

        public GetTaskUseCase(ITaskRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T> ExecuteAsync(int id, int userId)
            => await _repository.GetTaskAsync(id, userId);

    }
}
