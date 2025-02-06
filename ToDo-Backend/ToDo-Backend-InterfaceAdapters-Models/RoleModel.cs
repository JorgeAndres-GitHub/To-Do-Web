using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_InterfaceAdapters_Models
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public virtual ICollection<UserModel> Users { get; set; }
    }
}
