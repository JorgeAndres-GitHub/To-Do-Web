using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCase;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases;
using ToDo_Backend_FrameworksDrivers_API.Configuration;
using ToDo_Backend_FrameworksDrivers_API.Services;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.User;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.UserRequests;

namespace ToDo_Backend_FrameworksDrivers_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly RegisterUseCase<UserRegistrationRequestDTO, AuthResult> _registerUseCase;
        private readonly LoginUseCase<AuthResult> _loginUseCase;
        private readonly JwtConfig _jwtConfig;

        public AuthenticationController(RegisterUseCase<UserRegistrationRequestDTO, AuthResult> registerUseCase, LoginUseCase<AuthResult> loginUseCase, IOptions<JwtConfig> jwtConfig)
        {
            _registerUseCase = registerUseCase;
            _loginUseCase = loginUseCase;
            _jwtConfig = jwtConfig.Value;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO registerRequest)
        {
            await _registerUseCase.ExecuteAsync(registerRequest);
            return Ok(new AuthResult
            {
                Result = true
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO loginRequest)
        {
            var success = await _loginUseCase.ExecuteAsync(loginRequest.Email, loginRequest.Password);
            if(!success.Result)
                return BadRequest(success);

            var token = GenerateTokenService.GenerateToken(success.User, _jwtConfig);
            success.Token = token;

            return Ok(success);
        }
    }
}
