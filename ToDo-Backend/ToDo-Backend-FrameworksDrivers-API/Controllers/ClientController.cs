using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Views;
using ToDo_Backend_CA_InterfaceAdapters_Data;
using ToDo_Backend_FrameworksDrivers_API.Services;
using ToDo_Backend_FrameworksDrivers_API.Validators.UserValidators;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.UserRequests;

namespace ToDo_Backend_FrameworksDrivers_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly GetProfileViewModelUseCase<UserEntity, AuthResult, UserViewModel> _getProfileViewModelUseCase;
        private readonly UpdateProfileUseCase<AuthResult> _updateProfileUseCase;
        private readonly GetProfileUseCase<UserEntity, AuthResult> _getProfileUseCase;

        public ClientController(GetProfileViewModelUseCase<UserEntity, AuthResult, UserViewModel> getProfileViewModelUseCase, 
            UpdateProfileUseCase<AuthResult> updateProfileUseCase,
            GetProfileUseCase<UserEntity, AuthResult> getProfileUseCase)
        {
            _getProfileViewModelUseCase = getProfileViewModelUseCase;
            _getProfileUseCase = getProfileUseCase;
            _updateProfileUseCase = updateProfileUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserIdService.GetUserId(User);

            var user = await _getProfileViewModelUseCase.ExecuteAsync(userId);
            return Ok(user);
        }

        // ... existing code ...

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] JsonPatchDocument<UpdateUserRequestDTO> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var userId = GetUserIdService.GetUserId(User);

            var user = await _getProfileUseCase.ExecuteAsync(userId);

            if (user == null)
                return NotFound("User not found");

            var updateUserRequest = new UpdateUserRequestDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                IdentificationNumber = user.IdentificationNumber,
                Country = user.Country,
                City = user.City,
                Phone = user.Phone,
                Email = user.Email,
            };

            patchDoc.ApplyTo(updateUserRequest, ModelState);

            var validator = new UpdateUserValidator();
            var validationResult = validator.Validate(updateUserRequest);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            // Update the user with the patched values
            user.FirstName = updateUserRequest.FirstName;
            user.LastName = updateUserRequest.LastName;
            user.IdentificationNumber = updateUserRequest.IdentificationNumber;
            user.Country = updateUserRequest.Country;
            user.City = updateUserRequest.City;
            user.Phone = updateUserRequest.Phone;
            user.Email = updateUserRequest.Email;

            await _updateProfileUseCase.ExecuteAsync(user);
            return NoContent();
        }
    }
}
