using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo_Backend_CA_AplicationLayer.Interfaces.User
{
    public interface IAccountRepository<TEntity, TOutput>
    {
        Task<TOutput> GetUserById(int id);
        Task<TOutput> GetUserByEmail(string email);
        Task CreateUserAsync(TEntity user);
        Task<TOutput> LoginAsync(string email, string password);
        Task<TOutput> UpdateUser(TEntity user);
        Task<TOutput> DeleteUser(int id);
    }
}
