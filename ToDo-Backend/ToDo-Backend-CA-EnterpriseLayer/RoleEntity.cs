using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_CA_EnterpriseLayer
{
    public class RoleEntity
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public virtual ICollection<UserEntity> Users { get; set; }
    }
}
