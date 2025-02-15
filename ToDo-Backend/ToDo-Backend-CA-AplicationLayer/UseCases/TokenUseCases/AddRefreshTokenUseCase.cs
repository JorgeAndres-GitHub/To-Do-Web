using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.TokenUseCases
{
    public class AddRefreshTokenUseCase<TModel, TAuthResult>
    {
        private readonly ITokenRepository<TModel, TAuthResult> _tokenRepository;

        public AddRefreshTokenUseCase(ITokenRepository<TModel, TAuthResult> repository)
        {
            _tokenRepository = repository;
        }

        public async Task<TAuthResult> ExecuteAsync(string tokenId, int userId)
            =>  await _tokenRepository.AddRefreshTokenAsync(tokenId, userId);
    }
}
