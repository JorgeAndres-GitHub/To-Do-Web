using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;

namespace ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCase
{
    public class RegisterUseCase<TDTO, TOutput>
    {
        private readonly IAccountRepository<UserEntity, TOutput> _accountRepository;
        private readonly IMapper<TDTO, UserEntity> _accountMapper;

        public RegisterUseCase(IAccountRepository<UserEntity, TOutput> accountRepository, IMapper<TDTO, UserEntity> accountMapper) 
        {
            _accountRepository = accountRepository;
            _accountMapper = accountMapper;
        }
        
        public async Task<TOutput> ExecuteAsync(TDTO registerRequest)
        {
            var userEntity = _accountMapper.ToEntity(registerRequest);
            return await _accountRepository.CreateUserAsync(userEntity);
        }
         
    }
}
