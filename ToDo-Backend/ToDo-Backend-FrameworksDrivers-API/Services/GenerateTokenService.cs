using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases;
using ToDo_Backend_FrameworksDrivers_API.Configuration;
using ToDo_Backend_FrameworksDrivers_API.Services.Common;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Models;

namespace ToDo_Backend_FrameworksDrivers_API.Services
{
    public static class GenerateTokenService
    {
        internal static async Task<AuthResult> GenerateToken(AddRefreshTokenUseCase<AuthResult> useCase, UserModel user, JwtConfig jwtConfig)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                })),
                Expires = DateTime.UtcNow.Add(jwtConfig.ExpiryTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshTokenResult = await useCase.ExecuteAsync(token.Id, user.Id);

            refreshTokenResult.Token = jwtToken;

            return refreshTokenResult;
        }
    }
}
