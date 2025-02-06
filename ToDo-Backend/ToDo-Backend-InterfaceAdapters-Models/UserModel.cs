using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_InterfaceAdapters_Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentificationNumber { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int CreatedTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PublishedTasks { get; set; }
        public bool? IsEmailConfirmed { get; set; }
        public string? VerificationCode { get; set; }
        public string? UpdateConfirmationCode { get; set; }
        public int IdRol { get; set; }

        public virtual RoleModel Role { get; set; }
        public virtual ICollection<UserTaskModel> UserTasks { get; set; }
    }
}
