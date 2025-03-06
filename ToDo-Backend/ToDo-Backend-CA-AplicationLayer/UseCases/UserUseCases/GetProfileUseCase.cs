using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases
{
    public class GetProfileUseCase<TEntity, TAuthenticationOutput>
    {
        private readonly IAccountRepository<TEntity, TAuthenticationOutput> _repository;

        public GetProfileUseCase(IAccountRepository<TEntity, TAuthenticationOutput> repository)
        {
            _repository = repository;
        }

        public async Task<TEntity> ExecuteAsync(int id) => await _repository.GetUserByIdAsync(id);        
    }
}
