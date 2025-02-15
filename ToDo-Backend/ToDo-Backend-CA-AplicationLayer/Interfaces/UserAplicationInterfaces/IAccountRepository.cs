using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.Interfaces.User
{
    public interface IAccountRepository<TEntity, TAuthenticationOutput>
    {
        Task<TEntity> GetUserById(int id);
        Task<TAuthenticationOutput> CreateUserAsync(TEntity user);
        Task<TAuthenticationOutput> LoginAsync(string email, string password);
        Task UpdateUserAsync(TEntity id);
        Task DeleteUserAsync(int id);
    }
}
