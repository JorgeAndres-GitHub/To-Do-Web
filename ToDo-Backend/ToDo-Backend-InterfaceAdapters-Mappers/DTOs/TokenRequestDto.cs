using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_InterfaceAdapters_Mappers.DTOs
{
    public class TokenRequestDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
