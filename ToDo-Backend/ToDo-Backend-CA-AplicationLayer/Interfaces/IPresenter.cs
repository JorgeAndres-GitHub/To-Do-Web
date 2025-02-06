using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_CA_AplicationLayer.Interfaces
{
    public interface IPresenter<TEntity, TOutput>
    {
        public IEnumerable<TOutput> Present(IEnumerable<TEntity> entities);
    }
}
