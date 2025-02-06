using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_CA_AplicationLayer.Exceptions
{
    public class InvalidUserCreationException : Exception
    {
        public InvalidUserCreationException(string message) : base(message) { }
    }
}
