using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.TokenUseCases
{
    public class UpdateRefreshTokenUseCase<TModel, TAuthResult>
    {
        private readonly ITokenRepository<TModel, TAuthResult> _repository;

        public UpdateRefreshTokenUseCase(ITokenRepository<TModel, TAuthResult> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(int idToken) => await _repository.UpdateRefreshTokenAsync(idToken);
    }
}
