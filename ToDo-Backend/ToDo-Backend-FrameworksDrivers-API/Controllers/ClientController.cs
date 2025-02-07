using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Views;
using ToDo_Backend_FrameworksDrivers_API.Services;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.UserRequests;

namespace ToDo_Backend_FrameworksDrivers_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly GetProfileUseCase<UserEntity, AuthResult, UserViewModel> _getProfileUseCase;
        private readonly UpdateProfileUseCase<> _updateProfileUseCase;

        public ClientController(GetProfileUseCase<UserEntity, AuthResult, UserViewModel> getProfileUseCase, UpdateProfileUseCase updateProfileUseCase)
        {
            _getProfileUseCase = getProfileUseCase;
            _updateProfileUseCase = updateProfileUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserIdService.GetUserId(User);

            var user = await _getProfileUseCase.ExecuteAsync(userId);
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequestDTO request)
        {
            var userId = GetUserIdService.GetUserId(User);

            await _updateProfileUseCase.ExecuteAsync(userId);

            return NoContent();
        }
    }
}
