using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces
{
    public interface IAccountMapper<TDTO, TOutput>
    {
        public TOutput ToEntity(TDTO dto);
    }
}
