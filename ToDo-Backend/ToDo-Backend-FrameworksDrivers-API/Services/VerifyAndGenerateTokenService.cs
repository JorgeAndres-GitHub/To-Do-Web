using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ToDo_Backend_CA_AplicationLayer.Exceptions;
using ToDo_Backend_CA_AplicationLayer.UseCases.TokenUseCases;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs;
using ToDo_Backend_InterfaceAdapters_Models;

namespace ToDo_Backend_FrameworksDrivers_API.Services
{
    public static class VerifyAndGenerateTokenService
    {
        public static async Task<int> VerifyAndGenerateTokenAsync(TokenRequestDto dto, TokenValidationParameters tokenValidationParameters,
            GetRefreshTokenUseCase<RefreshTokenModel, AuthResult> getTokenUseCase, UpdateRefreshTokenUseCase<RefreshTokenModel, AuthResult> updateTokenUseCase)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            tokenValidationParameters.ValidateLifetime = false;

            var tokenBeingVerified = jwtTokenHandler.ValidateToken(dto.Token, tokenValidationParameters, out var validatedToken);
            
            if(validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                if (!result || tokenBeingVerified == null)
                    throw new ValidateTokenException("Invalid token");
            }

            var expClaim = tokenBeingVerified.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
            if (expClaim == null)
            {
                throw new ValidateTokenException("Token does not contain an expiration claim.");
            }

            var utcExpiryDate = long.Parse(expClaim.Value);


            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiryDate).UtcDateTime;
            if (expiryDate < DateTime.UtcNow)
                throw new ValidateTokenException("Expired token");

            var storedToken = await getTokenUseCase.ExecuteAsync(dto.RefreshToken);

            var jtiClaim = tokenBeingVerified.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
            if (jtiClaim?.Value == null)
            {
                throw new ValidateTokenException("Missing JTI claim.");
            }
            var jti = jtiClaim.Value;


            if (jti != storedToken.JwtId)
                throw new ValidateTokenException("Invalid Token");

            if (storedToken.ExpiryDate < DateTime.UtcNow)
                throw new ValidateTokenException("Expired Token");

            await updateTokenUseCase.ExecuteAsync(storedToken.Id);

            return storedToken.UserId;
        }
    }
}
