using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Exceptions;
using ToDo_Backend_CA_InterfaceAdapters_Data;
using ToDo_Backend_InterfaceAdapters_Models;
using ToDo_Backend_InterfaceAdapters_Repository;
using Xunit;

namespace ToDo_Backend_CA_TestUnit
{
    public class TokenRepositoryTest
    {
        private readonly AppDbContext _context;
        private readonly TokenRepository _tokenRepository;

        public TokenRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _tokenRepository = new TokenRepository(_context);
        }

        [Fact]
        public async Task AddRefreshTokenAsync_ShouldAddToken_WhenValidData()
        {
            // Arrange
            var tokenId = Guid.NewGuid().ToString();
            var userId = 1;

            // Act
            var result = await _tokenRepository.AddRefreshTokenAsync(tokenId, userId);

            // Assert
            Assert.True(result.Result);
            Assert.NotNull(result.RefreshToken);
        }

        [Fact]
        public async Task GetRefreshTokenAsync_ShouldReturnToken_WhenTokenExists()
        {
            // Arrange
            var tokenId = Guid.NewGuid().ToString();
            var userId = 1;
            var result = await _tokenRepository.AddRefreshTokenAsync(tokenId, userId);

            // Act
            var refreshToken = await _tokenRepository.GetRefreshTokenAsync(result.RefreshToken);

            // Assert
            Assert.NotNull(refreshToken);
            Assert.Equal(result.RefreshToken, refreshToken.Token);
        }

        [Fact]
        public async Task GetRefreshTokenAsync_ShouldThrowException_WhenTokenIsInvalid()
        {
            // Arrange
            var invalidToken = "invalidToken";

            // Act & Assert
            await Assert.ThrowsAsync<ValidateTokenException>(() => _tokenRepository.GetRefreshTokenAsync(invalidToken));
        }

        [Fact]
        public async Task UpdateRefreshTokenAsync_ShouldMarkTokenAsUsed_WhenTokenExists()
        {
            // Arrange
            var tokenId = Guid.NewGuid().ToString();
            var userId = 1;
            var result = await _tokenRepository.AddRefreshTokenAsync(tokenId, userId);
            var refreshToken = await _tokenRepository.GetRefreshTokenAsync(result.RefreshToken);

            // Verificar que el token no esté revocado ni usado
            Assert.False(refreshToken.IsRevoked);
            Assert.False(refreshToken.IsUsed);

            // Act
            await _tokenRepository.UpdateRefreshTokenAsync(refreshToken.Id);

            // Assert
            await Assert.ThrowsAsync<ValidateTokenException>(() => _tokenRepository.GetRefreshTokenAsync(result.RefreshToken));
        }

    }
}
