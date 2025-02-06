using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_CA_AplicationLayer.Exceptions
{
    public class TaskIdValidationException : Exception
    {
        public TaskIdValidationException() : base("Id not found.") { }

        public TaskIdValidationException(int id) : base($"Task with Id {id} not found.") { }

        public TaskIdValidationException(string message) : base(message) { }
    }
}
