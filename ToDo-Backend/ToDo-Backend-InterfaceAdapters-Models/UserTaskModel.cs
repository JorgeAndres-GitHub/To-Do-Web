using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_InterfaceAdapters_Models
{
    public class UserTaskModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public virtual UserModel User { get; set; }
        public virtual TaskModel Task { get; set; }
    }
}
