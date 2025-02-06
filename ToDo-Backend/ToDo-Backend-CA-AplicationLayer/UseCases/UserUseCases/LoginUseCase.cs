using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases
{
    public class LoginUseCase<TOutput>
    {
        private readonly IAccountRepository<UserEntity, TOutput> _accountRepository;

        public LoginUseCase(IAccountRepository<UserEntity, TOutput> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<TOutput> ExecuteAsync(string email, string password) => await _accountRepository.LoginAsync(email, password);
    }
}
