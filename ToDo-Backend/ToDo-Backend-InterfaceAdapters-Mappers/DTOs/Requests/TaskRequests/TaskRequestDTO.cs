using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task
{
    public class TaskRequestDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly? DueDate { get; set; }
    }
}
