using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.TokenUseCases
{
    public class GetRefreshTokenUseCase<TModel, TAuthResult>
    {
        private readonly ITokenRepository<TModel, TAuthResult> _repository;

        public GetRefreshTokenUseCase(ITokenRepository<TModel, TAuthResult> repository)
        {
            _repository = repository;
        }

        public async Task<TModel> ExecuteAsync(string refreshToken) => await _repository.GetRefreshTokenAsync(refreshToken);
    }
}
