using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task
{
    public class BulkDeleteRequestDto
    {
        public List<int> Ids { get; set; }
    }
}
