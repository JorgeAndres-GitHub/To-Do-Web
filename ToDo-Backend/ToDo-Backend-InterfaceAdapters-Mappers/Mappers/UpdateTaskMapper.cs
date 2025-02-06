using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task;

namespace ToDo_Backend_InterfaceAdapters_Mappers.Mappers
{
    public class UpdateTaskMapper : IMapper<UpdateTaskRequestDTO, TaskItem>
    {
        public TaskItem ToEntity(UpdateTaskRequestDTO dto) => new TaskItem
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate.HasValue ? dto.DueDate.Value.ToDateTime(TimeOnly.MinValue) : null
        };
    }
}
