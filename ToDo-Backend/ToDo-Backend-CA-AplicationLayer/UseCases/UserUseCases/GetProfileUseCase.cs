using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases
{
    public class GetProfileUseCase<TEntity, TAuthenticationOutput, TOutput>
    {
        private readonly IAccountRepository<TEntity, TAuthenticationOutput> _repository;
        private readonly IAccountPresenter<TEntity, TOutput> _presenter;

        public GetProfileUseCase(IAccountRepository<TEntity, TAuthenticationOutput> repository, IAccountPresenter<TEntity, TOutput> presenter)
        {
            _repository = repository;
            _presenter = presenter;
        }

        public async Task<TOutput> ExecuteAsync(int id)
        {
            var user = await _repository.GetUserById(id);
            return _presenter.Present(user);
        }
              
    }
}
