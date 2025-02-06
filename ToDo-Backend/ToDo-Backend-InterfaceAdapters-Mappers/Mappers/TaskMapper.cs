using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task;

namespace ToDo_Backend_InterfaceAdapters_Mappers.Mappers
{
    public class TaskMapper : IMapper<TaskRequestDTO, TaskItem>
    {
        public TaskItem ToEntity(TaskRequestDTO dto) => new TaskItem
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false,
            CreatedAt = DateTime.Now,
            DueDate = dto.DueDate.HasValue ? dto.DueDate.Value.ToDateTime(TimeOnly.MinValue) : null
        };
    }
}
