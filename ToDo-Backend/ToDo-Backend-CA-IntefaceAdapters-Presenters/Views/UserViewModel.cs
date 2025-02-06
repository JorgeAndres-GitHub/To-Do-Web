using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_CA_IntefaceAdapters_Presenters.Views
{
    public class UserViewModel
    {
        public string FullName { get; set; }
        public string IdentificationNumer { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CreatedTasks { get; set; }
        public string CompletedTasks { get; set; }
        public string PublishedTasks { get; set; }
    }
}
