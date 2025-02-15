using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Exceptions;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;
using ToDo_Backend_CA_InterfaceAdapters_Data;
using ToDo_Backend_FrameworksDrivers_API.Services.Common;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Models;

namespace ToDo_Backend_InterfaceAdapters_Repository
{
    public class TokenRepository : ITokenRepository<RefreshTokenModel, AuthResult>
    {
        private readonly AppDbContext _context;

        public TokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthResult> AddRefreshTokenAsync(string tokenId, int userId)
        {
            var refreshToken = new RefreshTokenModel
            {
                JwtId = tokenId,
                Token = RandomGenerator.GenerateRandomString(23),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsRevoked = false,
                IsUsed = false,
                UserId = userId
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResult
            {
                RefreshToken = refreshToken.Token,
                Result = true
            };
        }

        public async Task<RefreshTokenModel> GetRefreshTokenAsync(string refreshToken)
        {
            var refreshTokenModel = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (refreshTokenModel == null)
                throw new ValidateTokenException("Invalid Token");

            if (refreshTokenModel.IsRevoked || refreshTokenModel.IsUsed)
                throw new ValidateTokenException("Invalid Token");

            return refreshTokenModel;
        }

        public async Task UpdateRefreshTokenAsync(int idToken)
        {
            var refreshToken = await _context.RefreshTokens.FirstAsync(t => t.Id == idToken);
            refreshToken.IsUsed = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
