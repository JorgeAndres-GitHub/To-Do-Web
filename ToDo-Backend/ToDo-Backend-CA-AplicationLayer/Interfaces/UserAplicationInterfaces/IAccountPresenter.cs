using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces
{
    public interface IAccountPresenter<TEntity, TOutput>
    {
        public TOutput Present(TEntity entities);
    }
}
