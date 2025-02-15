using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases
{
    public class DeleteProfileUseCase<TAuthenticationOutput>
    {
        private readonly IAccountRepository<UserEntity, TAuthenticationOutput> _accountRepository;

        public DeleteProfileUseCase(IAccountRepository<UserEntity, TAuthenticationOutput> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task ExecuteAsync(int id) =>
            await _accountRepository.DeleteUserAsync(id);
    }
}
