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
    public class AddRefreshTokenUseCase<TRefreshToken, TAuthenticationOutput>
    {
        private readonly IAccountRepository<UserEntity, TAuthenticationOutput> _repository;
        private readonly IMapper<TRefreshToken, RefreshTokenEntity> _mapper;

        public AddRefreshTokenUseCase(IAccountRepository<UserEntity, TAuthenticationOutput> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(TRefreshToken refreshToken)
        {
            await _repository.AddRefreshTokenAsync(refreshToken);
        }
    }
}
