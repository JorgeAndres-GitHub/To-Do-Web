using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Views;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;

namespace ToDo_Backend_FrameworksDrivers_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly GetProfileUseCase<UserEntity, AuthResult, UserViewModel> _getProfileUseCase;

        public ClientController(GetProfileUseCase<UserEntity, AuthResult, UserViewModel> getProfileUseCase)
        {
            _getProfileUseCase = getProfileUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim == null)
                return Unauthorized("User not authenticated");

            int userId = int.Parse(userIdClaim.Value);

            var user = await _getProfileUseCase.ExecuteAsync(userId);
            return Ok(user);
        }
    }
}
