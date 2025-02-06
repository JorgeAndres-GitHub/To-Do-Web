using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_CA_EnterpriseLayer
{
    public class UserTaskEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public virtual UserEntity User { get; set; }
        public virtual TaskItem Task { get; set; }
    }
}
