using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_InterfaceAdapters_Models;

namespace ToDo_Backend_InterfaceAdapters_Mappers.Auth
{
    public class AuthResult
    {
        public UserModel User { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Result { get; set; }
        public List<string> Errors { get; set; }
    }
}
