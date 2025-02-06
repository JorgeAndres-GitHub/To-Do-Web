using FluentValidation;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.UserRequests;

namespace ToDo_Backend_FrameworksDrivers_API.Validators.UserValidators
{
    public class LoginValidator : AbstractValidator<UserLoginRequestDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email must be valid");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}
