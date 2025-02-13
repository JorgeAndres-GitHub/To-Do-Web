using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases
{
    public class AddRefreshTokenUseCase<TAuthenticationOutput>
    {
        private readonly IAccountRepository<UserEntity, TAuthenticationOutput> _repository;

        public AddRefreshTokenUseCase(IAccountRepository<UserEntity, TAuthenticationOutput> repository)
        {
            _repository = repository;
        }

        public async Task<TAuthenticationOutput> ExecuteAsync(string tokenId, int userId)
            =>  await _repository.AddRefreshTokenAsync(tokenId, userId);
    }
}
