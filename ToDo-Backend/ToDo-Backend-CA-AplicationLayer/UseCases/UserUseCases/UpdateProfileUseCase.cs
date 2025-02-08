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
    public class UpdateProfileUseCase<TAuthResult>
    {
        private readonly IAccountRepository<UserEntity, TAuthResult> _repository;

        public UpdateProfileUseCase(IAccountRepository<UserEntity, TAuthResult> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(UserEntity user) => await _repository.UpdateUser(user);       
        
    }
}
